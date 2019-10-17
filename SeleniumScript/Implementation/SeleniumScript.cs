namespace SeleniumScript.Implementation
{
  using Antlr4.Runtime;
  using Grammar;
  using Exceptions;
  using Interfaces;
  using System;
  using OpenQA.Selenium;

  public class SeleniumScript : ISeleniumScript, IDisposable
  {
    private readonly ISeleniumScriptLogger seleniumScriptLogger;
    private readonly ISeleniumScriptWebDriver seleniumScriptWebDriver;
    private readonly ISeleniumScriptVisitor seleniumScriptVisitor;
    private readonly SeleniumScriptSyntaxErrorListener seleniumScriptSyntaxErrorListener;

    public event LogEventHandler OnLogEntryWritten;

    public SeleniumScript(IWebDriver webDriver)
    {
      this.seleniumScriptLogger = new SeleniumScriptLogger();
      this.seleniumScriptWebDriver = new SeleniumScriptWebDriver(webDriver, seleniumScriptLogger);
      this.seleniumScriptVisitor = new SeleniumScriptVisitor(seleniumScriptWebDriver, seleniumScriptLogger);
      this.seleniumScriptSyntaxErrorListener = new SeleniumScriptSyntaxErrorListener(seleniumScriptLogger);

      this.OnLogEntryWritten += (log) => { };
      this.seleniumScriptLogger.OnLogEntryWritten += (log) => OnLogEntryWritten(log); 
    }

    public void Run(string script)
    {
      var seleniumScriptLexer = new SeleniumScriptLexer(new AntlrInputStream(script));
      var seleniumScriptParser = new SeleniumScriptParser(new CommonTokenStream(seleniumScriptLexer));
      seleniumScriptParser.AddErrorListener(seleniumScriptSyntaxErrorListener);
      
      try
      {
        seleniumScriptVisitor.Visit(seleniumScriptParser.executionUnit());
      }
      catch (SeleniumScriptVisitorException seleniumScriptVisitorException)
      {
        seleniumScriptLogger.Log(seleniumScriptVisitorException.Message, Enums.LogLevel.VisitorError);
      }
      catch (SeleniumScriptWebDriverException seleniumScriptWebDriverException)
      {
        seleniumScriptLogger.Log(seleniumScriptWebDriverException.Message, Enums.LogLevel.SeleniumError);
      }
      catch (Exception e)
      {
        seleniumScriptLogger.Log(e.Message, Enums.LogLevel.RuntimeError);
      }
    }

    public void Dispose()
    {
      seleniumScriptWebDriver.Close();
    }
  }
}
