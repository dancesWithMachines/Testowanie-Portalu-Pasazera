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

        public void Wait(int seconds) //Pr�bowali�my unika� jak ognia, ale bez tego dziej� si� rzeczy niestworzone
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
            departureInput.SendKeys("Gda�sk");
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
            Wait(2);
            IWebElement departutreList = driver.FindElement(By.Id("departureFrom_listbox"));
            IList<IWebElement> departureListElements = departutreList.FindElements(By.TagName("li"));
            foreach (IWebElement element in departureListElements)
                if (element.Text.Contains("Gda�sk G��wny"))
                    element.Click();
            Wait(2);
            IWebElement arrivalInput = driver.FindElement(By.Id("arrivalTo"));
            arrivalInput.Click();
            arrivalInput.SendKeys("Sop");
            Wait(2);
            IWebElement arrivalList = driver.FindElement(By.Id("arrivalTo_listbox"));
            IList<IWebElement> arrivalListElements = arrivalList.FindElements(By.TagName("li"));
            foreach (IWebElement element in arrivalListElements)
                if (element.Text.Contains("Sopot"))
                    element.Click();
            Wait(2);
            IWebElement date = driver.FindElement(By.Id("main-search__dateStart"));
            date.Click();
            Wait(2);
            IWebElement dateAndYear = driver.FindElement(By.ClassName("k-nav-fast"));
            while (!dateAndYear.Text.Contains("LIPIEC 2021"))
                driver.FindElement(By.ClassName("k-nav-next")).Click();
            Wait(2);
            IWebElement calendar = driver.FindElement(By.ClassName("k-calendar"));
            IWebElement table = calendar.FindElement(By.TagName("tbody"));
            IList <IWebElement> days = table.FindElements(By.TagName("a"));
            foreach (IWebElement day in days)
                if (day.GetAttribute("data-value").Contains("2021/6/1"))
                {
                    day.Click();
                    break;
                }                    
            Wait(2);
            IWebElement timePicker = driver.FindElement(By.Id("main-search__timeStart"));
            timePicker.Click();
            Wait(2);
            IWebElement picker = driver.FindElement(By.ClassName("phTimePickerHoursList"));
            IList<IWebElement> times = picker.FindElements(By.TagName("div"));
            foreach (IWebElement time in times)
                if (time.Text.Contains("15:00"))
                    time.Click();
            Wait(2);
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
        [Description("Sprawd� czy zostan� wyszukane jakiekolwiek poci�gi, z Tczewa do Ko�cierzyny, z po��czeniem bezpo�rednim")]
        public void TestForLackOfResults()
        {
            IWebElement departureInput = driver.FindElement(By.Id("departureFrom"));
            departureInput.Click();
            departureInput.SendKeys("Tczew");
            IWebElement arrivalInput = driver.FindElement(By.Id("arrivalTo"));
            arrivalInput.Click();
            arrivalInput.SendKeys("Ko�cierzyna");
            IWebElement dateStartInput = driver.FindElement(By.Id("main-search__dateStart"));
            dateStartInput.Click();
            IWebElement dateInput = driver.FindElement(By.Id("main-search__dateStart"));
            dateInput.Click();
            dateInput.SendKeys(Keys.Control + "a");
            dateInput.SendKeys(Keys.Delete);
            dateInput.SendKeys("05.08.2021");
            IWebElement timeInput = driver.FindElement(By.Id("main-search__timeStart"));
            timeInput.Click();
            timeInput.SendKeys(Keys.Control + "a");
            timeInput.SendKeys(Keys.Delete);
            timeInput.SendKeys("18:30");
            driver.FindElement(By.ClassName("btn-start-search")).Click();
            IWebElement searchResults;
            while (true)
                try
                {
                    searchResults = driver.FindElement(By.ClassName("search-results__container"));
                    break;
                }
                catch { }
            IWebElement noResultsDiv = null;
            try
            {
                noResultsDiv = driver.FindElement(By.ClassName("search-results-not-found"));
            }
            catch { }
            Assert.IsNotNull(noResultsDiv);
        }

        [Test]
        [Description("Sprawd� czy strona uniemo�liwi wyszukiwanie gdy usuniemy wszystkie klasy transportu")]
        public void TestForLackOfTransportClass()
        {
            IWebElement departureInput = driver.FindElement(By.Id("departureFrom"));
            departureInput.Click();
            departureInput.SendKeys("Gdynia G��wna");
            IWebElement arrivalInput = driver.FindElement(By.Id("arrivalTo"));
            arrivalInput.Click();
            arrivalInput.SendKeys("Malbork");
            IWebElement dateStartInput = driver.FindElement(By.Id("main-search__dateStart"));
            dateStartInput.Click();
            IWebElement dateInput = driver.FindElement(By.Id("main-search__dateStart"));
            dateInput.Click();
            dateInput.SendKeys(Keys.Control + "a");
            dateInput.SendKeys(Keys.Delete);
            dateInput.SendKeys("31.08.2021");
            IWebElement timeInput = driver.FindElement(By.Id("main-search__timeStart"));
            timeInput.Click();
            timeInput.SendKeys(Keys.Control + "a");
            timeInput.SendKeys(Keys.Delete);
            timeInput.SendKeys("13:13");
            IWebElement moreOptionsDiv = driver.FindElement(By.ClassName("main-search__more-options-trigger"));
            moreOptionsDiv.FindElement(By.TagName("button")).Click();
            IWebElement classList = driver.FindElement(By.Id("sl-klas"));
            actions.MoveToElement(classList);
            actions.Perform();
            IList<IWebElement> classes = classList.FindElements(By.TagName("li"));
            for (int i = classes.Count - 1; i >= 0; i--)
            {
                classes[i].FindElement(By.TagName("button")).Click();
                classes = classList.FindElements(By.TagName("li"));
            }               
            driver.FindElement(By.ClassName("btn-start-search")).Click();
            IWebElement errorMessage = null;
            try
            {
                errorMessage = driver.FindElement(By.ClassName("param-error"));
            } 
            catch { }
            Assert.IsNotNull(errorMessage);

        }

        [Test]
        [Description("Sprawd� czy mo�na dosta� si� z Tczewa do Gda�ska, za po�rednikiem SKM")]
        public void TestForSKMConnection()
        {
            IWebElement departureInput = driver.FindElement(By.Id("departureFrom"));
            departureInput.Click();
            departureInput.SendKeys("Tczew");
            IWebElement arrivalInput = driver.FindElement(By.Id("arrivalTo"));
            arrivalInput.Click();
            arrivalInput.SendKeys("Gda�sk G��wny");
            IWebElement dateStartInput = driver.FindElement(By.Id("main-search__dateStart"));
            dateStartInput.Click();
            IWebElement moreOptionsDiv = driver.FindElement(By.ClassName("main-search__more-options-trigger"));
            moreOptionsDiv.FindElement(By.TagName("button")).Click();
            IWebElement classList = driver.FindElement(By.Id("sl-carr"));
            actions.MoveToElement(classList);
            actions.Perform();
            IList<IWebElement> classes = classList.FindElements(By.TagName("li"));
            for (int i = classes.Count - 1; i >= 0; i--)
            {
                if (!classes[i].FindElement(By.TagName("span")).Text.Contains("Szybka Kolej Miejska"))
                    classes[i].FindElement(By.TagName("button")).Click();
                classes = classList.FindElements(By.TagName("li"));
            }
            driver.FindElement(By.ClassName("btn-start-search")).Click();
            IWebElement searchResults;
            while (true)
                try
                {
                    searchResults = driver.FindElement(By.ClassName("search-results__container"));
                    break;
                }
                catch { }
            IWebElement noResultsDiv = null;
            try
            {
                noResultsDiv = driver.FindElement(By.ClassName("search-results-not-found"));
            }
            catch { }
            Assert.IsNotNull(noResultsDiv);
        }

        [Test]
        [Description("Sprawd� czy pkp mo�e si� nie sp�nia�")] //Je�eli ten test nie przechodzi, to znaczy �e pkp tzyma poziom
        public void TestPKP()
        {
            IWebElement departureInput = driver.FindElement(By.Id("departureFrom"));
            departureInput.Click();
            departureInput.SendKeys("Pruszcz Gda�ski");
            IWebElement arrivalInput = driver.FindElement(By.Id("arrivalTo"));
            arrivalInput.Click();
            arrivalInput.SendKeys("Gda�sk");
            driver.FindElement(By.ClassName("btn-start-search")).Click();
            IWebElement searchResults;
            while (true)
                try
                {
                    searchResults = driver.FindElement(By.ClassName("search-results__container"));
                    break;
                }
                catch { }
            IWebElement searchResultContainer = driver.FindElement(By.ClassName("search-results__container"));
            IList<IWebElement> results = searchResultContainer.FindElements(By.ClassName("search-results__item"));
            bool wasLate = false;
            foreach (IWebElement result in results)
            {
                IList<IWebElement> allerts = result.FindElements(By.ClassName("color--alert"));
                if (allerts.Count > 0)
                    wasLate = true;
            }
            Assert.IsFalse(wasLate,"Koleje polskie trzymaj� poziom hyhy");
        }

        [Test]
        [Description("Po wyszukaniu, zmie� parametry pozostawiaj�c jako jednyego przewo�nika Arriv�")]
        public void TestChangingParameters()
        {
            IWebElement departureInput = driver.FindElement(By.Id("departureFrom"));
            departureInput.Click();
            departureInput.SendKeys("Tczew");
            IWebElement arrivalInput = driver.FindElement(By.Id("arrivalTo"));
            arrivalInput.Click();
            arrivalInput.SendKeys("Gda�sk G��wny");
            driver.FindElement(By.ClassName("btn-start-search")).Click();
            IWebElement searchResults;
            while (true)
                try
                {
                    searchResults = driver.FindElement(By.ClassName("search-results__container"));
                    break;
                }
                catch { }
            IWebElement headerInfo = driver.FindElement(By.ClassName("header-extra-info"));
            headerInfo.FindElement(By.TagName("button")).Click();
            IWebElement company = driver.FindElement(By.ClassName("company-summary"));
            company.FindElement(By.TagName("button")).Click();
            IWebElement searchOptionCompany = driver.FindElement(By.Id("searchOptionsCompany"));
            IList<IWebElement> companylabels = searchOptionCompany.FindElements(By.TagName("label"));
            foreach (IWebElement label in companylabels)
                if (!label.FindElement(By.TagName("span")).Text.Contains("Arriva") && !label.FindElement(By.TagName("span")).Text.Contains("wszyscy"))
                    label.FindElement(By.TagName("span")).Click();
            searchOptionCompany.FindElement(By.ClassName("options-submit")).Click();
            searchResults = null;
            while (true)
                try
                {
                    searchResults = driver.FindElement(By.ClassName("search-results__container"));
                    break;
                }
                catch { }
            IWebElement noResultsDiv = null;
            try
            {
                noResultsDiv = driver.FindElement(By.ClassName("search-results-not-found"));
            }
            catch { }
            Assert.IsNotNull(noResultsDiv);
        }

        [Test]
        [Description("Testowanie opcji szybkie po��czenia, dodaj�c 3 stacje")]

    }
}