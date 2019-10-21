namespace SeleniumScript.Implementation
{
  using global::SeleniumScript.Implementation.DataModel;
  using Grammar;
  using Interfaces;
  using static Grammar.SeleniumScriptParser;

  public partial class SeleniumScriptInterpreter : SeleniumScriptBaseVisitor<Symbol>, ISeleniumScriptInterpreter
  {
    private readonly ISeleniumScriptWebDriver webDriver;
    private readonly ISeleniumScriptLogger seleniumLogger;
    private readonly ICallStack callStack;

    public SeleniumScriptInterpreter(ISeleniumScriptWebDriver webDriver, ICallStack callStack, ISeleniumScriptLogger seleniumLogger)
    {
      this.webDriver = webDriver;
      this.callStack = callStack;
      this.seleniumLogger = seleniumLogger;
    }

    public event CallBackEventHandler OnCallback;

    public void Run(ExecutionUnitContext context)
    {
      base.Visit(context);
    }
  }
}
