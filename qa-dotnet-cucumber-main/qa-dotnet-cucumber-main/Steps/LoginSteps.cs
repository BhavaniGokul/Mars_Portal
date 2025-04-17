using OpenQA.Selenium;
using Reqnroll;
using NUnit.Framework;
using OpenQA.Selenium.Support.UI;
using qa_dotnet_cucumber.Pages;

namespace qa_dotnet_cucumber.Steps
{
    [Binding]
    public class LoginSteps
    {
        private readonly LoginPage _loginPage;
        private readonly NavigationHelper _navigationHelper;

        public LoginSteps(LoginPage loginPage, NavigationHelper navigationHelper)
        {
            _loginPage = loginPage;
            _navigationHelper = navigationHelper;
        }

        [Given("I am on the login page")]
        public void GivenIAmOnTheLoginPage()
        {
            _navigationHelper.NavigateTo("Home/");
            Assert.That(_loginPage.IsAtLoginPage(), Is.True, "Home");

        }

        [When("I enter valid credentials")]
        public void WhenIEnterValidCredentials()
        {
            _loginPage.Login("bha@gmail.com", "bhavani");
        }

        [When("I enter an invalid username and valid password")]
        public void WhenIEnterAnInvalidUsernameAndValidPassword()
        {
            _loginPage.Login("bhava", "bhavani");
        }

        [When("I enter a valid username and invalid password")]
        public void WhenIEnterAValidUsernameAndInvalidPassword()
        {
            _loginPage.Login("bha@gmail.com", "123");
        }

        [When("I enter empty credentials")]
        public void WhenIEnterEmptyCredentials()
        {
            _loginPage.Login("", "");
        }

        [Then("I should see the secure area")]
        public void ThenIShouldSeeTheSecureArea()
        {
            var successMessage = _loginPage.GetSuccessMessage();
            Assert.That(successMessage, Does.Contain("Mars Logo"), "Should see the Mars dashboard");
        }

        [Then("I should see an error message")]
        public void ThenIShouldSeeAnErrorMessage()
        {
            // Use LoginPage's driver to wait for and verify the error message
            var wait = new WebDriverWait(_loginPage.Driver, TimeSpan.FromSeconds(30));
            var errorMessageElement = wait.Until(d => d.FindElement(By.XPath("(//div[@class='field error ']//div)[1]")));
            var errorMessage = errorMessageElement.Text;
            
            Assert.That(errorMessage, Does.Match("Please enter a valid email address|Password must be at least 6 characters|Username is required"),
                "Should see an appropriate error message");
        }
    }
}