namespace SeleniumScript.Implementation
{
  using Antlr4.Runtime.Misc;
  using global::SeleniumScript.Enums;
  using global::SeleniumScript.Exceptions;
  using global::SeleniumScript.Grammar;
  using global::SeleniumScript.Implementation.DataModel;
  using global::SeleniumScript.Implementation.Enums;
  using System.Collections.Generic;
  using System.Linq;
  using static global::SeleniumScript.Grammar.SeleniumScriptParser;
 
  public partial class SeleniumScriptInterpreter : SeleniumScriptBaseVisitor<object>
  {
    public override object VisitFunctionArguments([NotNull] FunctionArgumentsContext context)
    {
      seleniumLogger.Log($"Resolving function arguments");
      return context.data().Select(x => (string)x.Accept(this)).ToArray();
    }

    public override object VisitFunctionCall([NotNull] FunctionCallContext context)
    {
      var identifier = context.IDENTIFIER().GetText();
      var functionArguments = context.functionArguments();
      var functionDefinition = callStack.ResolveFunction(identifier);
      var functionBody = functionDefinition.Body.statementBlock();

      seleniumLogger.Log($"Calling function {identifier}", SeleniumScriptLogLevel.InterpreterDetails);
      var argumentList = new Dictionary<string, string>();

      if (functionDefinition.Parameters.Count > 0 && (context.functionArguments() == null || context.functionArguments().data().Length != functionDefinition.Parameters.Count))
      {
        throw new SeleniumScriptVisitorException(
          $"Error calling function {identifier}, argument list does not match parameter definition\n" +
          $"Expecting: {string.Join(", ", functionDefinition.Parameters.Select(x => x.Value + " " + x.Key))}"
        );
      }
      else if (functionDefinition.Parameters.Count > 0)
      {
        argumentList = ((string[])functionArguments.Accept(this))
          .Select((x, i) => new KeyValuePair<string, string>(functionDefinition.Parameters.ElementAt(i).Key, x))
          .ToDictionary(x => x.Key, x => x.Value);
      }

      callStack.Push(StackFrameScope.Method);

      foreach (var kv in argumentList)
      {
        callStack.Current.AddVariable(kv.Key, kv.Value);
      }

      foreach (var statement in functionBody.statement())
      {
        if (statement.functionReturn() != null)
        {
          var data = statement.functionReturn().data().Accept(this);
          callStack.Pop();
          return data;
        }

        statement.Accept(this);
      }

      callStack.Pop();
      return null;
    }

    public override object VisitFunctionDeclaration([NotNull] FunctionDeclarationContext context)
    {
      var identifier = context.IDENTIFIER().GetText();
      var returnType = context.variableType() != null ? context.variableType().GetText() : null;
      seleniumLogger.Log($"Declaring function {identifier}", SeleniumScriptLogLevel.InterpreterDetails);

      var parameters = new Dictionary<string, string>();
      for (int i = 0; i < context.functionParameters().variableType().Length; i++)
      {
        parameters.Add(context.functionParameters().IDENTIFIER(i).GetText(), context.functionParameters().variableType(i).GetText());
      }

      var functionDefinition = new Function(identifier, returnType)
      {
        Body = context.functionBody(),
        Parameters = parameters
      };

      callStack.Current.AddFunction(identifier, functionDefinition);

      return null;
    }
  }
}
