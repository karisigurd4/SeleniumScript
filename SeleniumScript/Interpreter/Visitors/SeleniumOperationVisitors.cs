namespace SeleniumScript.Implementation
{
  using Antlr4.Runtime.Misc;
  using global::SeleniumScript.Enums;
  using global::SeleniumScript.Exceptions;
  using global::SeleniumScript.Grammar;
  using static global::SeleniumScript.Grammar.SeleniumScriptParser;

  public partial class SeleniumScriptInterpreter : SeleniumScriptBaseVisitor<object>
  {
    public override object VisitOperationCallBack([NotNull] OperationCallBackContext context)
    {
      var data = (string)context.parameterList().data(0).Accept(this);

      seleniumLogger.Log($"Executing callback {data}", SeleniumScriptLogLevel.InterpreterDetails);
      OnCallback(data);
      return null;
    }

    public override object VisitOperationGetUrl([NotNull] OperationGetUrlContext context)
    {
      seleniumLogger.Log($"Calling selenium WebDriver GetUrl()", SeleniumScriptLogLevel.InterpreterDetails);
      return webDriver.GetUrl();
    }

    public override object VisitOperationLog([NotNull] OperationLogContext context)
    {
      var data = (string)context.parameterList().data(0).Accept(this);

      seleniumLogger.Log($"Logging to Script log level: {data}", SeleniumScriptLogLevel.InterpreterDetails);
      seleniumLogger.Log(data, SeleniumScriptLogLevel.Script);
      return null;
    }

    public override object VisitOperationGetElementText([NotNull] OperationGetElementTextContext context)
    {
      var xPath = (string)Visit(context.parameterList().data(0));
      var elementDescription = (string)ResolveoptionalParameter(context.parameterList().data(), 1);

      seleniumLogger.Log($"Calling GetElementText on driver with xpath {xPath} and description {elementDescription}", SeleniumScriptLogLevel.InterpreterDetails);
      return webDriver.GetElementText(xPath, elementDescription);
    }

    public override object VisitOperationSendkeys([NotNull] OperationSendkeysContext context)
    {
      var xPath = (string)Visit(context.parameterList().data(0));
      var data = (string)Visit(context.parameterList().data(1));
      var elementDescription = (string)ResolveoptionalParameter(context.parameterList().data(), 2);

      seleniumLogger.Log($"Calling sendKeys on driver with xpath {xPath} and data {data} and description {elementDescription}", SeleniumScriptLogLevel.InterpreterDetails);
      webDriver.SendKeys(xPath, data, elementDescription);
      return null;
    }

    public override object VisitOperationClick([NotNull] OperationClickContext context)
    {
      var xPath = (string)Visit(context.parameterList().data(0));
      var elementDescription = (string)ResolveoptionalParameter(context.parameterList().data(), 1);

      seleniumLogger.Log($"Calling click on driver with xpath {xPath} and description {elementDescription}", SeleniumScriptLogLevel.InterpreterDetails);
      webDriver.Click(xPath, elementDescription);
      return null;
    }

    public override object VisitOperationNavigateTo([NotNull] OperationNavigateToContext context)
    {
      var url = (string)Visit(context.parameterList().data(0));

      seleniumLogger.Log($"Navigating driver to {url}", SeleniumScriptLogLevel.InterpreterDetails);
      webDriver.NavigateTo(url);
      return null;
    }

    public override object VisitOperationWait([NotNull] OperationWaitContext context)
    {
      seleniumLogger.Log($"Operation wait", SeleniumScriptLogLevel.InterpreterDetails);

      int numberOfSeconds;
      if (!int.TryParse((string)Visit(context.parameterList().data(0)), out numberOfSeconds))
      {
        throw new SeleniumScriptVisitorException($"Number could not be parsed");
      }
      else
      {
        webDriver.WaitForSeconds(numberOfSeconds);
      }
      return null;
    }
  }
}
