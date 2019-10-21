namespace SeleniumScript.Implementation
{
  using Antlr4.Runtime;
  using Grammar;
  using Exceptions;
  using Interfaces;
  using System;
  using global::SeleniumScript.Factories;
  using global::SeleniumScript.Enums;
  using System.Collections.Generic;

  public class SeleniumScript : ISeleniumScript, IDisposable
  {
    private readonly ISeleniumScriptLogger seleniumScriptLogger;
    private readonly ISeleniumScriptWebDriver seleniumScriptWebDriver;
    private readonly ISeleniumScriptInterpreter seleniumScriptVisitor;
    private readonly Dictionary<string, Action> callbackHandlers = new Dictionary<string, Action>();

    public event LogEventHandler OnLogEntryWritten;

    private Dictionary<string, SeleniumScriptLogLevel> exceptionLogLevels = new Dictionary<string, SeleniumScriptLogLevel>()
    {
      { typeof(SeleniumScriptException).Name, SeleniumScriptLogLevel.SeleniumScriptError },
      { typeof(SeleniumScriptSyntaxException).Name, SeleniumScriptLogLevel.SyntaxError },
      { typeof(SeleniumScriptVisitorException).Name, SeleniumScriptLogLevel.VisitorError },
      { typeof(SeleniumScriptWebDriverException).Name, SeleniumScriptLogLevel.WebDriverError },
      { typeof(Exception).Name, SeleniumScriptLogLevel.RuntimeError }
    };

    public SeleniumScript(OpenQA.Selenium.IWebDriver webDriver)
    {
      this.seleniumScriptLogger = new SeleniumScriptLogger();
      seleniumScriptLogger.OnLogEntryWritten += (log) => OnLogEntryWritten(log); 

      this.seleniumScriptWebDriver = new SeleniumScriptWebDriver(webDriver, seleniumScriptLogger);

      var callStack = new CallStack(new StackFrameHandlerFactory(), seleniumScriptLogger);
      this.seleniumScriptVisitor = new SeleniumScriptInterpreter(seleniumScriptWebDriver, callStack, seleniumScriptLogger);
      this.seleniumScriptVisitor.OnCallback += (callback) => HandleCallback(callback);

      this.OnLogEntryWritten += (log) => { };
    }

    public void Run(string script)
    {
      var seleniumScriptLexer = new SeleniumScriptLexer(new AntlrInputStream(script));
      var seleniumScriptParser = new SeleniumScriptParser(new CommonTokenStream(seleniumScriptLexer));
      seleniumScriptParser.AddErrorListener(new SeleniumScriptSyntaxErrorListener());
      
      try
      {
        seleniumScriptVisitor.Run(seleniumScriptParser.executionUnit());
      }
      catch (Exception e)
      {
        seleniumScriptLogger.Log(e.Message, exceptionLogLevels[e.GetType().Name]);
        Dispose();
        throw e;
      }
    }

    public void RegisterCallbackHandler(string callBackName, Action action)
    {
      callbackHandlers.Add(callBackName, action);
    }

    private void HandleCallback(string callback)
    {
      if (!callbackHandlers.ContainsKey(callback))
      {
        throw new SeleniumScriptException($"Callback with name {callback} has not been registered");
      }

      callbackHandlers[callback]();
    }

    public void Dispose()
    {
      seleniumScriptWebDriver.Close();
    }
  }
}
