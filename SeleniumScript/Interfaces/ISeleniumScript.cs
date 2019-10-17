namespace SeleniumScript.Interfaces
{
  using Contracts;

  public interface ISeleniumScript
  {
    event LogEventHandler OnLogEntryWritten;
    void Run(string script);
  }
}
