namespace SeleniumScript.Implementation
{
  using Antlr4.Runtime.Misc;
  using global::SeleniumScript.Enums;
  using global::SeleniumScript.Exceptions;
  using global::SeleniumScript.Grammar;
  using global::SeleniumScript.Implementation.DataModel;
  using global::SeleniumScript.Interpreter.Enums;
  using System;
  using static global::SeleniumScript.Grammar.SeleniumScriptParser;

  public partial class SeleniumScriptInterpreter : SeleniumScriptBaseVisitor<Symbol>
  {
    public override Symbol VisitOperationRandom([NotNull] OperationRandomContext context)
    {
      var random = new Random();
      Symbol first = null, second = null;

      first = context.first.Accept(this);

      if(context.second != null)
      {
        second = context.second.Accept(this);

        return new Symbol(string.Empty, ReturnType.Int, random.Next(first.AsInt, second.AsInt));
      }

      return new Symbol(string.Empty, ReturnType.Int, random.Next(first.AsInt));
    }

    public override Symbol VisitOperationCallBack([NotNull] OperationCallBackContext context)
    {
      var data = context.parameterList().data(0).Accept(this).AsString;

      seleniumLogger.Log($"Executing callback {data}", SeleniumScriptLogLevel.InterpreterDetails);

      OnCallback(data);
      return null;
    }

    public override Symbol VisitOperationGetUrl([NotNull] OperationGetUrlContext context)
    {
      seleniumLogger.Log($"Calling selenium WebDriver GetUrl()", SeleniumScriptLogLevel.InterpreterDetails);
      return new Symbol(string.Empty, ReturnType.String, webDriver.GetUrl());
    }

    public override Symbol VisitOperationLog([NotNull] OperationLogContext context)
    {
      var data = context.parameterList().data(0).Accept(this).AsString;

      seleniumLogger.Log($"Logging to Script log level: {data}", SeleniumScriptLogLevel.InterpreterDetails);
      seleniumLogger.Log(data, SeleniumScriptLogLevel.Script);
      return null;
    }

    public override Symbol VisitOperationGetElementText([NotNull] OperationGetElementTextContext context)
    {
      var xPath = context.parameterList().data(0).Accept(this).AsString;
      var elementDescription = ResolveoptionalParameter(context.parameterList().data(), 1).AsString;

      seleniumLogger.Log($"Calling GetElementText on driver with xpath {xPath} and description {elementDescription}", SeleniumScriptLogLevel.InterpreterDetails);
      return new Symbol(string.Empty, ReturnType.String, webDriver.GetElementText(xPath, elementDescription));
    }

    public override Symbol VisitOperationSendkeys([NotNull] OperationSendkeysContext context)
    {
      var xPath = context.parameterList().data(0).Accept(this).AsString;
      var data = context.parameterList().data(1).Accept(this).AsString;
      var elementDescription = ResolveoptionalParameter(context.parameterList().data(), 2).AsString;

      seleniumLogger.Log($"Calling sendKeys on driver with xpath {xPath} and data {data} and description {elementDescription}", SeleniumScriptLogLevel.InterpreterDetails);
      webDriver.SendKeys(xPath, data, elementDescription);
      return null;
    }

    public override Symbol VisitOperationClick([NotNull] OperationClickContext context)
    {
      var xPath = context.parameterList().data(0).Accept(this).AsString;
      var elementDescription = ResolveoptionalParameter(context.parameterList().data(), 1).AsString;

      seleniumLogger.Log($"Calling click on driver with xpath {xPath} and description {elementDescription}", SeleniumScriptLogLevel.InterpreterDetails);
      webDriver.Click(xPath, elementDescription);
      return null;
    }

    public override Symbol VisitOperationNavigateTo([NotNull] OperationNavigateToContext context)
    {
      var url = context.parameterList().data(0).Accept(this).AsString;

      seleniumLogger.Log($"Navigating driver to {url}", SeleniumScriptLogLevel.InterpreterDetails);
      webDriver.NavigateTo(url);
      return null;
    }

    public override Symbol VisitOperationWait([NotNull] OperationWaitContext context)
    {
      seleniumLogger.Log($"Operation wait", SeleniumScriptLogLevel.InterpreterDetails);
      webDriver.WaitForSeconds(context.parameterList().data(0).Accept(this).AsInt);
      return null;
    }
  }
}
