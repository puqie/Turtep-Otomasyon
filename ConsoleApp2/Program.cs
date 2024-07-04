using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--disable-javascript");
            chromeOptions.AddArgument("--silent");
            chromeOptions.AddArgument("start-maximized");
            chromeOptions.AddArgument("--window-size=5920,5080");
            chromeOptions.AddArgument("--log-level=3");
            chromeOptions.AddArgument("--disable-blink-features=AutomationControlled");
            chromeOptions.AddArgument("user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, Gecko) Chrome/87.0.4280.88 Safari/537.36");
            chromeOptions.AddArguments("--force-device-scale-factor=0.20");
            //chromeOptions.AddArguments("--headless");

            var driver = new ChromeDriver(chromeOptions);

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(6));
            string link = "https://oys.yesevi.edu.tr/";
            string courseNo = "";
            HashSet<string> uniqueText = new HashSet<string>();
            StringBuilder text = new StringBuilder();
            driver.Url = link;
            Console.Clear();
            driver.Manage().Window.Maximize();

            Login(driver, wait);
            NavigateToCourses(driver);
            int courseCount = GetCourseCount(driver);
            DisplayCourses(driver, courseCount);

            Console.WriteLine("\nPlease select a course: ");
            courseNo = Console.ReadLine();
            Console.Clear();

            string courseName = SelectCourse(driver, wait, courseNo);
            CollectCourseContent(driver, wait, uniqueText);
            CreatePdf(courseName, uniqueText);

            driver.Quit();
            Environment.Exit(0);
        }

        static void Login(IWebDriver driver, WebDriverWait wait)
        {
            driver.FindElement(By.Name("kullanici_adi")).SendKeys("your_username");
            driver.FindElement(By.Name("sifre")).SendKeys("your_password");
            driver.FindElement(By.XPath("/html/body/div[6]/div/div[1]/form/div[3]/div/a[1]")).Click();
        }

        static void NavigateToCourses(IWebDriver driver)
        {
            try
            {
                driver.FindElement(By.XPath("/html/body/div[3]/div[2]/div/div[2]/div/div[2]/div/div[2]/div[2]/div[3]/div/div/div[3]/button")).Click();
            }
            catch { }
            driver.FindElement(By.XPath("/html/body/div[3]/div[1]/div/div/div[2]/div/div/ul/li[4]/a/span")).Click();
            driver.FindElement(By.XPath("/html/body/div[3]/div[1]/div/div/div[2]/div/div/ul/li[4]/a")).Click();
        }

        static int GetCourseCount(IWebDriver driver)
        {
            IList<IWebElement> courseElements = driver.FindElements(By.XPath("/html/body/div[3]/div[2]/div/div[2]/div/div[2]/div/div[2]/div/div/div[2]/div/div/div/div[1]/div/div"));
            return courseElements.Count;
        }

        static void DisplayCourses(IWebDriver driver, int courseCount)
        {
            for (int x = 1; x <= courseCount; x++)
            {
                Console.WriteLine($"{x}. {driver.FindElement(By.XPath($"/html/body/div[3]/div[2]/div/div[2]/div/div[2]/div/div[2]/div/div/div[2]/div/div/div/div[1]/div/div[{x}]/div/div/div/div[1]/p")).Text}");
            }
        }

        static string SelectCourse(IWebDriver driver, WebDriverWait wait, string courseNo)
        {
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath($"/html/body/div[3]/div[2]/div/div[2]/div/div[2]/div/div[2]/div/div/div[2]/div/div/div/div[1]/div/div[{courseNo}]/div/div/div/div[1]/p")));
            string courseName = driver.FindElement(By.XPath($"/html/body/div[3]/div[2]/div/div[2]/div/div[2]/div/div[2]/div/div/div[2]/div/div/div/div[1]/div/div[{courseNo}]/div/div/div/div[1]/p")).Text;
            Console.WriteLine($"Course Name: {courseName}\n");
            driver.FindElement(By.XPath($"/html/body/div[3]/div[2]/div/div[2]/div/div[2]/div/div[2]/div/div/div[2]/div/div/div/div[1]/div/div[{courseNo}]/div/div/div/div[3]/div[2]/span[2]/button")).Click();
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("/html/body/div[3]/div[2]/div/div[2]/div/div[2]/div/div[3]/div/div/div[2]/div/table/tbody/tr/td[5]/a")));
            driver.FindElement(By.XPath("/html/body/div[3]/div[2]/div/div[2]/div/div[2]/div/div[3]/div/div/div[2]/div/table/tbody/tr/td[5]/a")).Click();

            string existingWindowHandle = driver.CurrentWindowHandle;
            string popupHandle = string.Empty;
            ReadOnlyCollection<string> windowHandles = driver.WindowHandles;
            foreach (string handle in windowHandles)
            {
                if (handle != existingWindowHandle)
                {
                    popupHandle = handle; break;
                }
            }
            driver.SwitchTo().Window(popupHandle);

            return courseName;
        }

        static void CollectCourseContent(IWebDriver driver, WebDriverWait wait, HashSet<string> uniqueText)
        {
            wait.Until(webDriver => driver.FindElement(By.XPath("/html/body/div[3]/div/div[1]/div/div/div/div/div[2]/form/div/div/div/ul/li[1]")).Displayed);
            TraverseContent(driver, wait, uniqueText, "/html/body/div[3]/div/div[1]/div/div/div/div/div[2]/form/div/div/div/ul/li", 1);
        }

        static void TraverseContent(IWebDriver driver, WebDriverWait wait, HashSet<string> uniqueText, string xpath, int depth)
        {
            IList<IWebElement> elements = driver.FindElements(By.XPath(xpath));
            int elementCount = elements.Count;

            for (int i = 1; i <= elementCount; i++)
            {
                string currentXpath = $"{xpath}[{i}]";
                try
                {
                    driver.FindElement(By.XPath(currentXpath)).Click();
                }
                catch (Exception)
                {
                    continue;
                }

                ProcessContent(driver, wait, uniqueText);

                TraverseContent(driver, wait, uniqueText, $"{currentXpath}/ul/li", depth + 1);
            }
        }

        static void ProcessContent(IWebDriver driver, WebDriverWait wait, HashSet<string> uniqueText)
        {
            try
            {
                wait.Until(webDriver => driver.FindElement(By.Id("frame-dersicerik")).Displayed);
                driver.SwitchTo().Frame("frame-dersicerik");
                wait.Until(webDriver => driver.FindElement(By.XPath("/html/body")).Displayed);

                IList<IWebElement> newSizeList = driver.FindElements(By.XPath("/html/body/ul/li"));
                if (newSizeList.Count != 0)
                {
                    for (int nsl = 1; nsl <= newSizeList.Count; nsl++)
                    {
                        driver.FindElement(By.XPath($"/html/body/ul/li[{nsl}]/a")).Click();
                        string content = driver.FindElement(By.ClassName("tab-content")).Text;
                        if (uniqueText.Add(content))
                        {
                            Console.WriteLine("New content added.");
                        }
                    }
                }
                else
                {
                    string content = driver.FindElement(By.ClassName("tab-content")).Text;
                    if (uniqueText.Add(content))
                    {
                        Console.WriteLine("New content added.");
                    }
                }
            }
            catch { }
            finally
            {
                driver.SwitchTo().DefaultContent();
            }
        }

        static void CreatePdf(string courseName, HashSet<string> uniqueText)
        {
            Console.WriteLine("\nCreating PDF. This may take a few minutes.");

            // Create document
            iTextSharp.text.Document document = new iTextSharp.text.Document();

            // Create a font that supports Turkish characters
            iTextSharp.text.pdf.BaseFont baseFont = iTextSharp.text.pdf.BaseFont.CreateFont("Helvetica", "CP1254", iTextSharp.text.pdf.BaseFont.NOT_EMBEDDED);
            iTextSharp.text.Font fontNormal = new iTextSharp.text.Font(baseFont, 12, iTextSharp.text.Font.NORMAL);

            string directory = "C:\\Turtep";

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Save the document to a file
            using (FileStream fs = new FileStream(Path.Combine(directory, $"{courseName}.pdf"), FileMode.Create))
            {
                iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, fs);
                document.Open();

                // Add content to the document
                foreach (var content in uniqueText)
                {
                    document.Add(new Paragraph(content, fontNormal));
                }

                document.Close();
            }

            Console.WriteLine("\nPDF is ready.");
            Console.WriteLine($"C:\\Turtep\\{courseName}.pdf");
            Console.WriteLine("Press any key to close.");
            Console.ReadKey();
        }
    }
}
