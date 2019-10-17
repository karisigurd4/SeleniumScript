namespace SeleniumScript.Implementation
{
  using global::SeleniumScript.Contracts;
  using global::SeleniumScript.Enums;
  using global::SeleniumScript.Interfaces;
  using System;

  public class SeleniumScriptLogger : ISeleniumScriptLogger
  {
    public event LogEventHandler OnLogEntryWritten;

    public void Log(string message, LogLevel logSeverity = LogLevel.SeleniumInfo)
    {
      var logEntry = new LogEntry()
      {
        LogLevel = logSeverity,
        Message = message,
        TimeStamp = DateTime.UtcNow
      };

      OnLogEntryWritten?.Invoke(logEntry);
    }
  }
}
