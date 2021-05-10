using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Threading;

namespace PortalPasazeraTest
{
    //Lista test�w
    //Sprawdzi� czy dzia�a przycisk zmiany kontrastu
    //Sprawdzi� czy wyszukuje poci�g z gda�ska do sopotu na podstawie godziny odjazu, pierwszego czerwca o godzine 10:30
    //Sprawdzi� czy wyszuka poci�g z sopotu do pruszcza gda�skiego na podstawie godziny przyjazzdu
    //Sprawdzi� czy jest jaki� sp�niony poci�g z malborka do gdyni
    //Sprawdzi� czy nie istnieje po��czenie bezpo�rednie Tczew-Ko�cierzyna
    //Sprawdzi� czy rozk�ad zmienia si� pomi�dzy pi�tkiem a sobot�



    public class Tests
    {
        IWebDriver driver;
        Actions actions;
        string url = "https://portalpasazera.pl/";

        public void AcceptCookies()
        {
            while (true)
            {
                try
                {
                    driver.FindElement(By.ClassName("allow-all-submit")).Click();
                    break;
                }
                catch { }
            }
        }

        public void wait(int seconds) //Pr�bowali�my unika� jak ognia, ale bez tego dziej� si� rzeczy niestworzone
        {
            Thread.Sleep(seconds * 1000);
        }

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            actions = new Actions(driver);
            driver.Manage().Window.Maximize();
            driver.Url = url;
            AcceptCookies();
        }    

        [TearDown]
        public void Teardown()
        {
            driver.Close();
        }

        [Test]
        [Description("Sprawd� czy dzia�a przycisk wysokiego kontrastu")]
        public void TestHighContrastButton()
        {
            string htmlClassNamesBefore = driver.FindElement(By.TagName("html")).GetAttribute("class").ToString();
            driver.FindElement(By.ClassName("btn-contrast")).Click();
            string htmlClassNamesAfter = driver.FindElement(By.TagName("html")).GetAttribute("class").ToString();
            Assert.Multiple(() => {
                Assert.AreNotEqual(htmlClassNamesBefore, htmlClassNamesAfter);
                Assert.IsTrue(htmlClassNamesAfter.Contains("highcontrast"));
            });

        }

        [Test]
        [Description("Sprawd� czy zostan� wyszukane jakiekolwiek poci�gi z Gda�ska do Sopotu o 10:30 pierwszego czerwca")]
        public void TestTrainAvailabilityFromGdanskToSopot()
        {

            IWebElement departureInput = driver.FindElement(By.Id("departureFrom"));
            departureInput.Click();
            departureInput.SendKeys("Gda�sk G��wny");
            IWebElement arrivalInput = driver.FindElement(By.Id("arrivalTo"));
            arrivalInput.Click();
            arrivalInput.SendKeys("Sopot");
            IWebElement dateStartInput = driver.FindElement(By.Id("main-search__dateStart"));
            dateStartInput.Click();
            IWebElement dateInput = driver.FindElement(By.Id("main-search__dateStart"));
            dateInput.Click();
            dateInput.SendKeys(Keys.Control + "a");
            dateInput.SendKeys(Keys.Delete);
            dateInput.SendKeys("01.06.2021");
            IWebElement timeInput = driver.FindElement(By.Id("main-search__timeStart"));
            timeInput.Click();
            timeInput.SendKeys(Keys.Control + "a");
            timeInput.SendKeys(Keys.Delete);
            timeInput.SendKeys("10:30");
            driver.FindElement(By.ClassName("btn-start-search")).Click();
            IWebElement searchResults;
            while (true)
                try
                {
                    searchResults = driver.FindElement(By.ClassName("search-results__container"));
                    break;
                }
                catch { }
            IList<IWebElement> listOfResults = searchResults.FindElements(By.ClassName("search-results__item"));
            Assert.Greater(listOfResults.Count, 0);

        }

        [Test]
        [Description("Sprawd� czy zostan� wyszukane jakiekolwiek poci�gi z Sopotu do Gda�ska o 15:00 pierwszego lipca, u�ywaj�c sugerowania")]
        public void TestTrainAvailabilityFromGdanskToSopotUsingAutocomplete()
        {

            IWebElement departureInput = driver.FindElement(By.Id("departureFrom"));
            departureInput.Click();
            departureInput.SendKeys("Gda");
            wait(2);
            IWebElement departutreList = driver.FindElement(By.Id("departureFrom_listbox"));
            IList<IWebElement> departureListElements = departutreList.FindElements(By.TagName("li"));
            foreach (IWebElement element in departureListElements)
                if (element.Text.Contains("Gda�sk G��wny"))
                    element.Click();
            wait(2);
            IWebElement arrivalInput = driver.FindElement(By.Id("arrivalTo"));
            arrivalInput.Click();
            arrivalInput.SendKeys("Sop");
            wait(2);
            IWebElement arrivalList = driver.FindElement(By.Id("arrivalTo_listbox"));
            IList<IWebElement> arrivalListElements = arrivalList.FindElements(By.TagName("li"));
            foreach (IWebElement element in arrivalListElements)
                if (element.Text.Contains("Sopot"))
                    element.Click();
            wait(2);
            IWebElement date = driver.FindElement(By.Id("main-search__dateStart"));
            date.Click();
            wait(2);
            IWebElement dateAndYear = driver.FindElement(By.ClassName("k-nav-fast"));
            while (!dateAndYear.Text.Contains("LIPIEC 2021"))
                driver.FindElement(By.ClassName("k-nav-next")).Click();
            wait(2);
            IWebElement calendar = driver.FindElement(By.ClassName("k-calendar"));
            IWebElement table = calendar.FindElement(By.TagName("tbody"));
            IList <IWebElement> days = table.FindElements(By.TagName("a"));
            foreach (IWebElement day in days)
                if (day.GetAttribute("data-value").Contains("2021/6/1"))
                {
                    day.Click();
                    break;
                }                    
            wait(2);
            IWebElement timePicker = driver.FindElement(By.Id("main-search__timeStart"));
            timePicker.Click();
            wait(2);
            IWebElement picker = driver.FindElement(By.ClassName("phTimePickerHoursList"));
            IList<IWebElement> times = picker.FindElements(By.TagName("div"));
            foreach (IWebElement time in times)
                if (time.Text.Contains("15:00"))
                    time.Click();
            wait(2);
            driver.FindElement(By.ClassName("btn-start-search")).Click();
            IWebElement searchResults;
            while (true)
                try
                {
                    searchResults = driver.FindElement(By.ClassName("search-results__container"));
                    break;
                }
                catch { }
            IList<IWebElement> listOfResults = searchResults.FindElements(By.ClassName("search-results__item"));
            Assert.Greater(listOfResults.Count, 0);

        }
    }
}