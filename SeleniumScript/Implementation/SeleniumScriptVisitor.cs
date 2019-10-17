namespace SeleniumScript.Implementation
{
  using Antlr4.Runtime.Misc;
  using Enums;
  using Exceptions;
  using Grammar;
  using Interfaces;
  using System.Collections.Generic;
  using static Grammar.SeleniumScriptParser;

  public class SeleniumScriptVisitor : SeleniumScriptBaseVisitor<object>, ISeleniumScriptVisitor
  {
    private readonly ISeleniumScriptWebDriver webDriver;
    private readonly ISeleniumScriptLogger seleniumLogger;

    public Dictionary<string, string> DeclaredVariables { get; } = new Dictionary<string, string>();

    public SeleniumScriptVisitor(ISeleniumScriptWebDriver webDriver, ISeleniumScriptLogger seleniumLogger)
    {
      this.webDriver = webDriver;
      this.seleniumLogger = seleniumLogger;
    }

    public void Visit(ExecutionUnitContext context)
    {
      base.Visit(context);
    }

    public override object VisitOperationGetUrl([NotNull] OperationGetUrlContext context)
    {
      return webDriver.GetUrl();
    }

    private object ParseOptionalParameter(DataContext[] context, int parameterIndex)
    {
      if (context == null || parameterIndex >= context.Length)
      {
        return string.Empty;
      }

      var data = Visit(context[parameterIndex]);

      if (data == null)
      {
        return string.Empty;
      }

      return data;
    }

    public override object VisitIfCondition([NotNull] IfConditionContext context)
    {
      if ((bool)VisitComparison(context.comparison()))
      {
        VisitStatementBlock(context.statementBlock()[0]);
        return true;
      }
      else
      {
        if(context.ELSE() != null && context.ELSE().Length == 1 && context.statementBlock().Length == 2)
        {
          VisitStatementBlock(context.statementBlock()[1]);
          return true;
        }
      }

      return base.VisitIfCondition(context);
    }

    public override object VisitComparison([NotNull] ComparisonContext context)
    {
      var first = (string)Visit(context.data()[0]);
      string second = string.Empty;
      if (context.data().Length == 2)
      {
        second = (string)Visit(context.data()[1]);
      }
      else
      {
        second = ((bool)VisitComparison(context.comparison()[0])) ? first : string.Empty;
      }
      seleniumLogger.Log($"Comparing {first} and {second} for equality.", LogLevel.VisitorDetails);
      return (bool)first.Equals(second);
    }

    public override object VisitOperationLog([NotNull] OperationLogContext context)
    {
      var data = (string)Visit(context.parameterList().data()[0]);
      seleniumLogger.Log($"Logging {data} to Script log level.", LogLevel.VisitorDetails);
      seleniumLogger.Log(data, LogLevel.Script);
      return base.VisitOperationLog(context);
    }

    public override object VisitOperationGetElementText([NotNull] OperationGetElementTextContext context)
    {
      string xPath = (string)Visit(context.parameterList().data()[0]);
      string elementDescription = (string)ParseOptionalParameter(context.parameterList().data(), 1);
      seleniumLogger.Log($"Calling GetElementText on driver with xpath {xPath} and description {elementDescription}", LogLevel.VisitorDetails);
      return webDriver.GetElementText(xPath, elementDescription);
    }

    public override object VisitOperationSendkeys([NotNull] OperationSendkeysContext context)
    {
      string xPath = (string)Visit(context.parameterList().data()[0]);
      string data = (string)Visit(context.parameterList().data()[1]);
      string elementDescription = (string)ParseOptionalParameter(context.parameterList().data(), 2);
      seleniumLogger.Log($"Calling sendKeys on driver with xpath {xPath} and data {data} and description {elementDescription}", LogLevel.VisitorDetails);
      webDriver.SendKeys(xPath, data, elementDescription);
      return base.VisitOperationSendkeys(context);
    }

    public override object VisitOperationClick([NotNull] OperationClickContext context)
    {
      string xPath = (string)Visit(context.parameterList().data()[0]);
      string elementDescription = (string)ParseOptionalParameter(context.parameterList().data(), 1);
      seleniumLogger.Log($"Calling click on driver with xpath {xPath} and description {elementDescription}", LogLevel.VisitorDetails);
      webDriver.Click(xPath, elementDescription);
      return base.VisitOperationClick(context);
    }

    public override object VisitOperationNavigateTo([NotNull] OperationNavigateToContext context)
    {
      string url = (string)Visit(context.parameterList().data()[0]);
      seleniumLogger.Log($"Navigating driver to {url}", LogLevel.VisitorDetails);
      webDriver.NavigateTo(url);
      return base.VisitOperationNavigateTo(context);
    }

    public override object VisitOperationWait([NotNull] OperationWaitContext context)
    {
      int numberOfSeconds;
      if (!int.TryParse((string)Visit(context.parameterList().data()[0]), out numberOfSeconds))
      {
        throw new SeleniumScriptVisitorException($"Number could not be parsed");
      }
      else
      {
        webDriver.WaitForSeconds(numberOfSeconds);
      }
      return base.VisitOperationWait(context);
    }

    public override object VisitVariableDeclaration([NotNull] VariableDeclarationContext context)
    {
      string identifier = context.variableAssignment().IDENTIFIER().GetText();
      if (DeclaredVariables.ContainsKey(identifier))
      {
        throw new SeleniumScriptVisitorException($"Redeclaration of identifier {identifier}");
      }
      seleniumLogger.Log($"Declaring variable {identifier}", LogLevel.VisitorDetails);
      DeclaredVariables.Add(identifier, string.Empty);
      return base.VisitVariableDeclaration(context);
    }

    public override object VisitVariableAssignment([NotNull] VariableAssignmentContext context)
    {
      string identifier = ResolveIdentifier(context.IDENTIFIER().GetText());
      object data = Visit(context.data());
      seleniumLogger.Log($"Assigning {data} to variable {identifier}", LogLevel.VisitorDetails);
      DeclaredVariables[identifier] = (string)data;
      return base.VisitVariableAssignment(context);
    }

    public override object VisitResolveIntLiteral([NotNull] ResolveIntLiteralContext context)
    {
      string data = context.INTLITERAL().GetText();
      seleniumLogger.Log($"Resolved int: {data}", LogLevel.VisitorDetails);
      return data;
    }

    public override object VisitResolveStringLiteral([NotNull] ResolveStringLiteralContext context)
    {
      string data = context.STRINGLITERAL().GetText();
      seleniumLogger.Log($"Resolved string: {data}", LogLevel.VisitorDetails);
      return data.Substring(1, data.Length - 2);
    }

    public override object VisitResolveReference([NotNull] ResolveReferenceContext context)
    {
      string identifier = ResolveIdentifier(context.IDENTIFIER().GetText());
      string data = DeclaredVariables[identifier];
      seleniumLogger.Log($"Resolved variable {identifier} value: {data}", LogLevel.VisitorDetails);
      return data;
    }

    private string ResolveIdentifier(string identifier)
    {
      if (!DeclaredVariables.ContainsKey(identifier))
      {
        throw new SeleniumScriptVisitorException($"Identifier {identifier} could not be resolved");
      }
      return identifier;
    }
  }
}
