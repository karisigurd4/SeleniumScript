namespace SeleniumScript.Implementation
{
  using Antlr4.Runtime.Misc;
  using global::SeleniumScript.Exceptions;
  using global::SeleniumScript.Grammar;
  using global::SeleniumScript.Implementation.DataModel;
  using global::SeleniumScript.Interpreter.Enums;
  using System;
  using static global::SeleniumScript.Grammar.SeleniumScriptParser;

  public partial class SeleniumScriptInterpreter : SeleniumScriptBaseVisitor<Symbol>
  {
    public override Symbol VisitVariableDeclaration([NotNull] VariableDeclarationContext context)
    {
      var variableType = context.variableType().Accept(this).ReturnType;
      var identifier = context.IDENTIFIER().GetText();
      callStack.Current.AddVariable(identifier, variableType, context?.data()?.Accept(this).AsString);
      return null;
    }

    public override Symbol VisitVariableAssignment([NotNull] VariableAssignmentContext context)
    {
      var identifier = context.IDENTIFIER().GetText();
      var data = context.data().Accept(this).AsString;
      callStack.Current.SetVariable(identifier, data);
      return null;
    }

    public override Symbol VisitVariableType([NotNull] VariableTypeContext context)
    {
      ReturnType variableType;
      if (!Enum.TryParse(context.GetText(), true, out variableType))
        return null;

      return new Symbol(string.Empty, variableType);
    }
  }
}
