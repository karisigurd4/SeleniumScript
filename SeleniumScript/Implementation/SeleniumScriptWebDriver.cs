namespace SeleniumScript.Implementation
{
  using OpenQA.Selenium;
  using OpenQA.Selenium.Chrome;
  using global::SeleniumScript.Enums;
  using global::SeleniumScript.Exceptions;
  using global::SeleniumScript.Interfaces;
  using System;
  using System.Text;
  using System.Threading;

  public class SeleniumScriptWebDriver : ISeleniumScriptWebDriver
  {
    private readonly IWebDriver webDriver;
    private readonly ISeleniumScriptLogger seleniumLogger;

    public SeleniumScriptWebDriver(IWebDriver webDriver, ISeleniumScriptLogger seleniumLogger)
    {
      this.webDriver = webDriver;
      this.seleniumLogger = seleniumLogger;
    }

    public void Close()
    {
      seleniumLogger.Log($"Closing and quitting webdriver", Enums.LogLevel.SeleniumInfo);
      webDriver.Close();
      webDriver.Quit();
    }

    public void Click(string xPath, string elementDescription)
    {
      seleniumLogger.Log($"Clicking element {elementDescription}", Enums.LogLevel.SeleniumInfo);
      Retry(() => webDriver.FindElement(By.XPath(xPath)).Click());
    }

    public void NavigateTo(string url)
    {
      seleniumLogger.Log($"Navigating driver to {url}");
      webDriver.Url = url;
    }

    public void SendKeys(string xPath, string data, string elementDescription)
    {
      seleniumLogger.Log($"Sending keys {data} to {elementDescription}", Enums.LogLevel.SeleniumInfo);
      Retry(() => webDriver.FindElement(By.XPath(xPath)).SendKeys(data));
    }

    public void WaitForSeconds(int numberOfSeconds)
    {
      seleniumLogger.Log($"Waiting for {numberOfSeconds} seconds", Enums.LogLevel.SeleniumInfo);
      Thread.Sleep(numberOfSeconds * 1000);
    }

    public string GetElementText(string xPath, string elementDescription)
    {
      seleniumLogger.Log($"Finding text on element {elementDescription}", Enums.LogLevel.SeleniumInfo);
      var elementText = Retry(() => webDriver.FindElement(By.XPath(xPath)).Text);
      return Encoding.UTF8.GetString(Encoding.Default.GetBytes(elementText));
    }

    public string GetUrl()
    {
      var url = webDriver.Url;
      seleniumLogger.Log($"Getting driver's URL: {url}", Enums.LogLevel.SeleniumInfo);
      return url;
    }

    private void Retry(Action action)
    {
      int timeout = 30;
      while (timeout > 0)
      {
        try
        {
          action();
          return;
        }
        catch (Exception)
        {
          seleniumLogger.Log("Operation not successful, retrying...", Enums.LogLevel.SeleniumInfo);
          Thread.Sleep(1000);
        }
      }

      throw new SeleniumScriptWebDriverException($"Operation could not be performed after {timeout} retries");
    }

    private T Retry<T>(Func<T> action)
    {
      int timeout = 30;
      while (timeout > 0)
      {
        try
        {
          return action();
        }
        catch (Exception)
        {
          seleniumLogger.Log("Operation not successful, retrying...", Enums.LogLevel.SeleniumInfo);
          Thread.Sleep(1000);
        }
      }

      throw new SeleniumScriptWebDriverException($"Operation could not be performed after {timeout} retries");
    }
  }
}
