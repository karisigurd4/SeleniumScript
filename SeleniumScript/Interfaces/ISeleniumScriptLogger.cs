namespace SeleniumScript.Interfaces
{
  using SeleniumScript.Enums;

  public interface ISeleniumScriptLogger
  {
    event LogEventHandler OnLogEntryWritten;
    void Log(string message, SeleniumScriptLogLevel logSeverity = SeleniumScriptLogLevel.SeleniumInfo);
  }
}
