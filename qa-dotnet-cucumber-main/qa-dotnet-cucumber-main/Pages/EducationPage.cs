using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using qa_dotnet_cucumber.Models;

namespace qa_dotnet_cucumber.Pages
{
    public class EducationPage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;
        public IWebDriver Driver => _driver;

        // Locators
        private readonly By EducationButton = By.XPath("//div[@class='ui fluid container']//a[3]");
        private readonly By AddNewButton = By.XPath("(//table[@class='ui fixed table'])[3]//div");
        private readonly By UniversityField = By.XPath("//input[@name='instituteName']");
        private readonly By CountryField = By.XPath("(//select[@class='ui dropdown'])[1]");
        private readonly By TitleField = By.XPath("(//select[@class='ui dropdown'])[2]");
        private readonly By DegreeField = By.XPath("//input[@name='degree']");
        private readonly By YearField = By.XPath("(//select[@class='ui dropdown'])[3]");
        private readonly By AddButton = By.XPath("(//input[@type='button'])[1]");
        private readonly By CancelButton = By.XPath("(//input[@type='button'])[2]");

        private readonly By EducationAddedMsg = By.XPath("//div[contains(text(),' Education has been added ')]");
       
        private readonly By EducationRow = By.XPath("(//table[@class='ui fixed table'])[3]//tbody");
       
        private readonly By UniversityInList = By.XPath("(//table[@class='ui fixed table'])[3]//tbody//tr/td[2]");
        private readonly By CountryInList = By.XPath("(//table[@class='ui fixed table'])[3]//tbody//tr/td[1]");
        private readonly By TitleInList = By.XPath("(//table[@class='ui fixed table'])[3]//tbody//tr/td[3]");
        private readonly By DegreeInList = By.XPath("(//table[@class='ui fixed table'])[3]//tbody//tr/td[4]");
        private readonly By YearInList = By.XPath("(//table[@class='ui fixed table'])[3]//tbody//tr/td[5]");

        //Delete Education Locators
        private readonly By EducationDeleteButton = By.XPath("//div[@data-tab='third']//i[@class='remove icon']");
        private readonly By EducationDeletedMsg = By.XPath("//div[contains(text(),'Education entry successfully removed')]");


        // Definining Constructor
        public EducationPage(IWebDriver driver) // Inject IWebDriver directly
        {
            _driver = driver;
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30)); // 10-second timeout
           
        }
        //Click Education Tab
        public void EducationTab()
        {
            try
            {
                // Click on Education Tab
                var educationButtonElement = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(EducationButton));
                educationButtonElement.Click();
            }
            catch (Exception ex)
            {
                Assert.Fail("Education Button has not been found");
            }
        }

        //******Adding New Education
        public void AddEducation(string university, string country, string title, string degree, string year)
        {   //Add Education through Add New
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

            //Enter University
            var UniversityElement = _wait.Until(ExpectedConditions.ElementIsVisible(UniversityField));
            UniversityElement.SendKeys(university);

            //Choose country from select dropdown
            var countryElement = _wait.Until(d => d.FindElement(CountryField));
            SelectElement countryUniversity = new SelectElement(countryElement);
            countryUniversity.SelectByText(country);

            //Choose title from select dropdown
            var titleElement = _wait.Until(d => d.FindElement(TitleField));
            SelectElement titleList = new SelectElement(titleElement);
            titleList.SelectByText(title);

            //Enter Degree
            var DegreeElement = _wait.Until(ExpectedConditions.ElementIsVisible(DegreeField));
            DegreeElement.SendKeys(degree);


            //Choose year from select dropdown
            var yearElement = _wait.Until(d => d.FindElement(YearField));
            SelectElement yearList = new SelectElement(yearElement);
            yearList.SelectByText(year);


            //Click Add button

            var AddButtonElement = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(AddButton));
            var AddButtonElementClickable = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(AddButton));
            AddButtonElementClickable.Click();
            Thread.Sleep(2000);
            
        }

        
        public string EducationAddedSuccessMsg()
        {
            var EduAddedMsg = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(EducationAddedMsg));
            return EduAddedMsg.Text;

        }
        public List<(string university, string country, string title, string degree, string year)> GetAllDisplayedEducationData()
        {
            var displayedDataList = new List<(string university, string country, string title, string degree, string year)>();

            //find all the rows from the table
            var rows = _driver.FindElements(EducationRow);
            
            foreach (var row in rows)
            {
                // Extract each piece of data for the row
                var university = row.FindElement(By.XPath(".//td[2]")).Text.Trim();
                var country = row.FindElement(By.XPath(".//td[1]")).Text.Trim();
                var title = row.FindElement(By.XPath(".//td[3]")).Text.Trim();
                var degree = row.FindElement(By.XPath(".//td[4]")).Text.Trim();
                var year = row.FindElement(By.XPath(".//td[5]")).Text.Trim();

                // Add the extracted data to the list
                displayedDataList.Add((university, country, title, degree, year));

               

                
            }
            return displayedDataList;
        }

        //*******Deleting Education entries
        public void DeleteAllEducation()
        {
            WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));
            while (true)
            {
                var deleteButtons = _driver.FindElements(EducationDeleteButton);

                if (deleteButtons.Count == 0)
                {
                    Console.WriteLine("All education entries are deleted.");
                    break;
                }
                int initialCount = deleteButtons.Count;
                try
                {

                    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(deleteButtons[0]));
                    deleteButtons[0].Click();
                    wait.Until(driver =>
                    {
                        var newDeleteButtons = driver.FindElements(EducationDeleteButton);
                        return newDeleteButtons.Count < initialCount;
                    });

                    Thread.Sleep(500);
                }
                catch (WebDriverTimeoutException ex)
                {
                    Console.WriteLine("Timeout waiting for delete action to complete: " + ex.Message);
                    break;
                }
            }
        }
        public bool AreEducationEntriesPresent()
        {
            var educationRows = _wait.Until(d => d.FindElements(EducationRow));
            return educationRows.Any();
        }

    }
}
