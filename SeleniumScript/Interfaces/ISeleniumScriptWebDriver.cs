namespace SeleniumScript.Interfaces
{
  public interface ISeleniumScriptWebDriver
  {
    void Click(string xPath, string elementDescription);
    void SendKeys(string xPath, string data, string elementDescription);
    void NavigateTo(string url);
    void WaitForSeconds(int numberOfSeconds);
    string GetElementText(string xPath, string elementDescription);
    string GetUrl();
    void Close();
  }
}
