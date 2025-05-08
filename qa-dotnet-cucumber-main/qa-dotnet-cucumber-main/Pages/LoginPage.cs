using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Reqnroll.BoDi;
// MUST USE with ExpectedConditions
using SeleniumExtras.WaitHelpers;

namespace qa_dotnet_cucumber.Pages
{
    public class LoginPage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;
        public IWebDriver Driver => _driver;

        public IWebElement signButton, userName, passWord, loginButton;

        // Locators
        private readonly By SigninButton= By.XPath("//*[@id=\"home\"]/div/div/div[1]/div/a");
        private readonly By UsernameField = By.XPath("//input[@name='email']");
        private readonly By PasswordField = By.XPath("//input[@name='password']");
        private readonly By LoginButton = By.XPath("/html/body/div[2]/div/div/div[1]/div/div[4]/button");
        private readonly By SuccessMessage = By.XPath("(//div[@id='account-profile-section']//a)[1]");

        public LoginPage(IWebDriver driver) // Inject IWebDriver directly
        {
            _driver = driver;
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30)); // 10-second timeout
            _driver.Navigate().GoToUrl(Hooks.Hooks.Settings.Environment.BaseUrl);
                       
        }

        public void Login(string username, string password)
        {
            //Click Signin button to login to the portal
            var signinButtonElement = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(SigninButton));
            signinButtonElement.Click();

            //Enter username as emailid
            var usernameElement = _wait.Until(ExpectedConditions.ElementIsVisible(UsernameField));
            usernameElement.SendKeys(username);

            //Enter password
            var passwordElement = _wait.Until(d => d.FindElement(PasswordField));
            passwordElement.SendKeys(password);


            //Click Login button
                       
            var loginButtonElement = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(LoginButton));
            var loginButtonElementClickable = _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(LoginButton));
            loginButtonElementClickable.Click();

         }

        public string GetSuccessMessage()
        {
            return _wait.Until(d => d.FindElement(SuccessMessage)).Text;
        }

        public bool IsAtLoginPage()
        {
            return _driver.Title.Contains("Home");
        }
    }
}