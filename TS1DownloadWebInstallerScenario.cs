using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EmsisoftChallenge
{
    [TestFixture]
    public class TS1DownloadWebInstallerScenario
    {
        private IWebDriver driver;
        private StringBuilder verificationErrors;
        private string baseURL;
        private string downloadLocation;

        [SetUp]
        public void SetupTest()
        {

            // Create an instance of the ChromeOptions class 
            ChromeOptions chromeOptions = new ChromeOptions();

            // Set Chrome download location
            downloadLocation = "C:\\Downloads\\emsisoft";

            //Set the arguments for Headless mode, Disable GPU acceleration, Set the download location and the automatic dowload parameter 
            chromeOptions.AddArgument("--headless");
            chromeOptions.AddArgument("--disable-gpu");
            chromeOptions.AddUserProfilePreference("download.default_directory", downloadLocation);
            chromeOptions.AddUserProfilePreference("profile.default_content_setting_values.automatic_downloads", 1);

            // Create ChromeDriver with options and set website url
            driver = new ChromeDriver(chromeOptions);
            baseURL = "https://emsisoft.com/en/anti-malware-home";

            verificationErrors = new StringBuilder();
        }

        [TearDown]
        public void TeardownTest()
        {
            try
            {
                driver.Quit();
            }
            catch (Exception)
            {
                // Ignore errors if unable to close the browser
            }
            NUnit.Framework.Assert.AreEqual("", verificationErrors.ToString());
        }

        [Test]
        public void TC1DownloadWebInstallerTest()
        {

            // Set Up the download location
            string expectedFilePath = downloadLocation + "\\EmsisoftAntiMalwareWebSetup.exe";
            bool fileExists = false;

            //Launch the Emsisoft Website
            driver.Navigate().GoToUrl(baseURL + "/");

            // Get all the links from the homePage and click on the desired link to downloads page
            IList<IWebElement> linksInHomePage = driver.FindElements(By.TagName("a"));
            linksInHomePage.First(element => element.Text == "Alternative installation options").Click();

            // Get all the links from the homePage and click on the download link
            IList<IWebElement> linksInDownloadPage = driver.FindElements(By.TagName("a"));
            linksInDownloadPage.First(element => element.Text == "Web installer").Click();

            // Try to download and check if file exists in the download folder
            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                wait.Until<bool>(x => fileExists = File.Exists(expectedFilePath));

                FileInfo fileInfo = new FileInfo(expectedFilePath);

                NUnit.Framework.Assert.AreEqual(fileInfo.Name, "EmsisoftAntiMalwareWebSetup.exe");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                // Delete the file after file download is checked
                if (File.Exists(expectedFilePath))
                    File.Delete(expectedFilePath);
            }

            // Close driver connection and check for errors
            TeardownTest();
        }
    }
}
