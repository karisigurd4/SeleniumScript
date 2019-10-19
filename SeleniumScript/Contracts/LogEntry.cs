namespace SeleniumScript.Contracts
{
  using SeleniumScript.Enums;
  using System;

  public class LogEntry
  {
    public string Message { get; set; }
    public SeleniumScriptLogLevel LogLevel { get; set; }
    public DateTime TimeStamp { get; set; }
  }
}
