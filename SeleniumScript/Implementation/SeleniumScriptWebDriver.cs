namespace SeleniumScript.Implementation
{
  using global::SeleniumScript.Enums;
  using global::SeleniumScript.Exceptions;
  using global::SeleniumScript.Interfaces;
  using OpenQA.Selenium;
  using System;
  using System.Text;
  using System.Threading;

  public class SeleniumScriptWebDriver : ISeleniumScriptWebDriver
  {
    private readonly IWebDriver webDriver;
    private readonly ISeleniumScriptLogger seleniumScriptLogger;

    public SeleniumScriptWebDriver(IWebDriver webDriver, ISeleniumScriptLogger seleniumLogger)
    {
      this.webDriver = webDriver;
      this.seleniumScriptLogger = seleniumLogger;
    }

    public void Close()
    {
      seleniumScriptLogger.Log($"Closing and quitting webdriver", SeleniumScriptLogLevel.SeleniumInfo);
      try
      {
        webDriver.Close();
      }
      catch (WebDriverException)
      {
        seleniumScriptLogger.Log("Exception occurred on webDriver.Close()", SeleniumScriptLogLevel.WebDriverError);
      }
      webDriver.Quit();
    }

    public void Click(string xPath, string elementDescription)
    {
      seleniumScriptLogger.Log($"Clicking element {elementDescription}", SeleniumScriptLogLevel.SeleniumInfo);
      Retry(() => webDriver.FindElement(By.XPath(xPath)).Click());
    }

    public void NavigateTo(string url)
    {
      seleniumScriptLogger.Log($"Navigating driver to {url}");
      webDriver.Url = url;
    }

    public void SendKeys(string xPath, string data, string elementDescription)
    {
      seleniumScriptLogger.Log($"Sending keys {data} to {elementDescription}", SeleniumScriptLogLevel.SeleniumInfo);
      Retry(() => webDriver.FindElement(By.XPath(xPath)).SendKeys(data));
    }

    public void WaitForSeconds(int numberOfSeconds)
    {
      seleniumScriptLogger.Log($"Waiting for {numberOfSeconds} seconds", SeleniumScriptLogLevel.SeleniumInfo);
      Thread.Sleep(numberOfSeconds * 1000);
    }

    public string GetElementText(string xPath, string elementDescription)
    {
      seleniumScriptLogger.Log($"Finding text on element {elementDescription}", SeleniumScriptLogLevel.SeleniumInfo);
      var elementText = Retry(() => webDriver.FindElement(By.XPath(xPath)).Text);
      return Encoding.UTF8.GetString(Encoding.Default.GetBytes(elementText));
    }

    public string GetUrl()
    {
      var url = webDriver.Url;
      seleniumScriptLogger.Log($"Getting driver's URL: {url}", SeleniumScriptLogLevel.SeleniumInfo);
      return url;
    }

    private void Retry(Action action)
    {
      int timeout = 30;
      while (timeout > 0)
      {
        try
        {
          Thread.Sleep(500);
          action();
          return;
        }
        catch (Exception)
        {
          seleniumScriptLogger.Log("Operation not successful, retrying...", SeleniumScriptLogLevel.SeleniumInfo);
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
          Thread.Sleep(500);
          return action();
        }
        catch (Exception)
        {
          seleniumScriptLogger.Log("Operation not successful, retrying...", SeleniumScriptLogLevel.SeleniumInfo);
          Thread.Sleep(1000);
        }
      }

      throw new SeleniumScriptWebDriverException($"Operation could not be performed after {timeout} retries");
    }
  }
}
