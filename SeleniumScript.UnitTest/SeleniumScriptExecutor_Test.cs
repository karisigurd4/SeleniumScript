﻿namespace SeleniumScript.UnitTest
{
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using OpenQA.Selenium.Chrome;
  using SeleniumScript.Contracts;
  using SeleniumScript.Exceptions;
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
        NavigateTo(""https://www.microsoft.com/sv-se/"");
        Click(""//button[@id = 'search']"", ""search button"");
        Log(""Finding the search bar and writing 'Windows 10' into it"");
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
          if(log.LogLevel == Enums.SeleniumScriptLogLevel.Script)
          {
            Debug.WriteLine($"{log.TimeStamp.ToString()} [{log.LogLevel.ToString()}] - {log.Message}");
          }
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

      Assert.AreEqual(1, logs.Where(x => x.LogLevel == Enums.SeleniumScriptLogLevel.VisitorError).Count());
      Assert.AreEqual("Number could not be parsed", logs.Where(x => x.LogLevel == Enums.SeleniumScriptLogLevel.VisitorError).First().Message);
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

      Assert.AreEqual(1, logs.Where(x => x.LogLevel == Enums.SeleniumScriptLogLevel.SyntaxError).Count());
      Assert.AreEqual("Line: 1, Char: 9 on value \"o\": no viable alternative at input 'stringa\"o\"'", logs.Where(x => x.LogLevel == Enums.SeleniumScriptLogLevel.SyntaxError).ToArray()[0].Message);
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

    [TestMethod]
    public void Can_Handle_Callback_Event()
    {
      string script = "Callback(\"callback\");";
      var logs = new List<LogEntry>();

      string callbackOutput = string.Empty;

      using (var seleniumScript = new SeleniumScript(new ChromeDriver(new ChromeOptions() { LeaveBrowserRunning = false })))
      {
        seleniumScript.RegisterCallbackHandler("callback", () => { callbackOutput = "Assigned"; });

        seleniumScript.Run(script);
      }

      Assert.AreEqual("Assigned", callbackOutput);
    }

    [TestMethod]
    public void Throws_Exception_If_Missing_Callback_Handler()
    {
      string script = "Callback(\"callback\");";
      string callbackOutput = string.Empty;

      var logs = new List<LogEntry>();
      using (var seleniumScript = new SeleniumScript(new ChromeDriver(new ChromeOptions() { LeaveBrowserRunning = false })))
      {
        Assert.ThrowsException<SeleniumScriptException>(() => seleniumScript.Run(script), "Callback with name callback has not been registered");
      }
    }
  }
}
