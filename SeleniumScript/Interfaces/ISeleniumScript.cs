namespace SeleniumScript.Interfaces
{
  using System;

  public interface ISeleniumScript
  {
    event LogEventHandler OnLogEntryWritten;
    void Run(string script);
    void RegisterCallbackHandler(string callBackName, Action action);
  }
}
