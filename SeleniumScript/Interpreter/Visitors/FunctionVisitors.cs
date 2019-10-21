namespace SeleniumScript.Implementation
{
  using Antlr4.Runtime.Misc;
  using global::SeleniumScript.Enums;
  using global::SeleniumScript.Exceptions;
  using global::SeleniumScript.Grammar;
  using global::SeleniumScript.Implementation.DataModel;
  using global::SeleniumScript.Implementation.Enums;
  using global::SeleniumScript.Interpreter.Enums;
  using System.Collections.Generic;
  using System.Linq;
  using static global::SeleniumScript.Grammar.SeleniumScriptParser;
 
  public partial class SeleniumScriptInterpreter : SeleniumScriptBaseVisitor<Symbol>
  {
    public override Symbol VisitFunctionArguments([NotNull] FunctionArgumentsContext context)
    {
      seleniumLogger.Log($"Resolving function arguments");
      return new Symbol(string.Empty, ReturnType.Array, context.data().Select(x => x.Accept(this).AsString).ToArray());
    }

    public override Symbol VisitFunctionCall([NotNull] FunctionCallContext context)
    {
      var identifier = context.IDENTIFIER().GetText();
      var functionArguments = context.functionArguments();
      var functionDefinition = callStack.ResolveFunction(identifier);
      var functionBody = functionDefinition.Body.statementBlock();

      seleniumLogger.Log($"Calling function {identifier}", SeleniumScriptLogLevel.InterpreterDetails);
      Variable[] argumentVariables = null;

      if (functionDefinition.Parameters.Count > 0 && (context.functionArguments() == null || context.functionArguments().data().Length != functionDefinition.Parameters.Count))
      {
        throw new SeleniumScriptVisitorException(
          $"Error calling function {identifier}, argument list does not match parameter definition\n" +
          $"Expecting: {string.Join(", ", functionDefinition.Parameters.Select(x => x.Value + " " + x.Key))}"
        );
      }
      else if (functionDefinition.Parameters.Count > 0)
      {
        argumentVariables = functionArguments.Accept(this).AsArray.Select((x, i) => new Variable(functionDefinition.Parameters.ElementAt(i).Key, functionDefinition.Parameters.ElementAt(i).Value, x)).ToArray();
      }

      callStack.Push(StackFrameScope.Method);

      if(argumentVariables != null)
      {
        foreach (var variable in argumentVariables)
        {
          callStack.Current.AddVariable(variable.Name, variable.ReturnType, variable.AsString);
        }
      }

      foreach (var statement in functionBody.statement())
      {
        if (statement.functionReturn() != null)
        {
          var data = statement.functionReturn().data().Accept(this).Value;
          callStack.Pop();
          return new Symbol(string.Empty, functionDefinition.ReturnType, data);
        }

        statement.Accept(this);
      }

      callStack.Pop();
      return null;
    }

    public override Symbol VisitFunctionDeclaration([NotNull] FunctionDeclarationContext context)
    {
      var identifier = context.IDENTIFIER().GetText();
      var returnType = context?.variableType()?.Accept(this).ReturnType;
      seleniumLogger.Log($"Declaring function {identifier}", SeleniumScriptLogLevel.InterpreterDetails);

      var parameters = new Dictionary<string, ReturnType>();
      for (int i = 0; i < context.functionParameters().variableType().Length; i++)
      {
        parameters.Add(context.functionParameters().IDENTIFIER(i).GetText(), context.functionParameters().variableType(i).Accept(this).ReturnType);
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
