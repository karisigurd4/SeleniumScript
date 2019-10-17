namespace SeleniumScript.UnitTest
{
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using OpenQA.Selenium.Chrome;
  using SeleniumScript.Contracts;
  using SeleniumScript.Implementation;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Linq;

  [TestClass]
  public class SeleniumScriptExecutor_Test
  {
    public SeleniumScriptExecutor_Test()
    {
    }

    [TestMethod]
    public void Can_Execute_Test_Script()
    {
      string script =
      @"
        string fakeEmailAddress = ""fakeemail@fakeemail"";
        Log(fakeEmailAddress);
        NavigateTo(""https://www.microsoft.com/sv-se/"");
        Log(""Navigating to microsoft!"");
        Click(""//button[@id = 'search']"");
        SendKeys(""//input[@id = 'cli_shellHeaderSearchInput']"", ""Windows 10"");
        Click(""//input[@id = 'cli_shellHeaderSearchInput']//following::a"", ""First search result"");
        SendKeys(""//div[@class = 'newsletter-email-main-section']//following::input"", fakeEmailAddress, ""Email address input field"");
        Wait(1);
        Click(""//div[@class = 'sfw-dialog']//following::div[@class = 'c-glyph glyph-cancel']"", ""Close dialog button"");
        string elementText = GetElementText(""//h1[@id = 'DynamicHeading_productTitle']"", ""Product title"");
        NavigateTo(""https://www.google.com/"");
        SendKeys(""//input[@name = 'q']"", elementText, ""Search bar"");
      ";

      using (var seleniumScript = new SeleniumScript(new ChromeDriver(new ChromeOptions() { LeaveBrowserRunning = false })))
      {
        seleniumScript.OnLogEntryWritten += (log) =>
        {
          Debug.WriteLine(log.Message);
        };
        seleniumScript.Run(script);
      }
    }

    [TestMethod]
    public void Logs_Error_On_Invalid_Script_Execution()
    {
      string script = "Wait(\"Hello world!\");";

      var logs = new List<LogEntry>();

      try
      {
        using (var seleniumScript = new SeleniumScript(new ChromeDriver(new ChromeOptions() { LeaveBrowserRunning = false })))
        {
          seleniumScript.OnLogEntryWritten += (log) => logs.Add(log);
          seleniumScript.Run(script);
        }
      }
      catch 
      {
      }

      Assert.AreEqual(1, logs.Where(x => x.LogLevel == Enums.LogLevel.VisitorError).Count());
      Assert.AreEqual("Number could not be parsed", logs.Where(x => x.LogLevel == Enums.LogLevel.VisitorError).First().Message);
    }

    [TestMethod]
    public void Reports_Syntax_Errors()
    {
      string script = "string a \"o\"";

      var logs = new List<LogEntry>();
      try
      {
        using (var seleniumScript = new SeleniumScript(new ChromeDriver(new ChromeOptions() { LeaveBrowserRunning = false })))
        {
          seleniumScript.OnLogEntryWritten += (log) => logs.Add(log);
          seleniumScript.Run(script);
        }
      }
      catch
      {
      }

      Assert.AreEqual(1, logs.Where(x => x.LogLevel == Enums.LogLevel.SyntaxError).Count());
      Assert.AreEqual("Line: 1, Char: 9 on value \"o\": missing '=' at '\"o\"'", logs.Where(x => x.LogLevel == Enums.LogLevel.SyntaxError).ToArray()[0].Message);
    }

    [TestMethod]
    public void Can_Run_Without_Event_Handler()
    {
      string script = "string a = \"o\";";

      var logs = new List<LogEntry>();
      using (var seleniumScript = new SeleniumScript(new ChromeDriver(new ChromeOptions() { LeaveBrowserRunning = false })))
      {
        seleniumScript.Run(script);
      }
    }
  }
}
