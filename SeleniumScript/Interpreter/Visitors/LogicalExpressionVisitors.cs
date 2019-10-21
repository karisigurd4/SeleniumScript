namespace SeleniumScript.Implementation
{
  using Antlr4.Runtime.Misc;
  using global::SeleniumScript.Enums;
  using global::SeleniumScript.Exceptions;
  using global::SeleniumScript.Grammar;
  using global::SeleniumScript.Implementation.DataModel;
  using global::SeleniumScript.Interpreter.Enums;
  using static global::SeleniumScript.Grammar.SeleniumScriptParser;

  public partial class SeleniumScriptInterpreter : SeleniumScriptBaseVisitor<Symbol>
  {
    public override Symbol VisitLogicalExpression([NotNull] LogicalExpressionContext context)
    {
      if (context.logicalExpression().Length > 0 && context.logicalExpression(0) != null)
      {
        var logicalOperator = context.logicalOperator(0).GetText();

        seleniumLogger.Log($"Visiting logical expression with operator {logicalOperator}", SeleniumScriptLogLevel.InterpreterDetails);

        switch (logicalOperator)
        {
          case "&&": return new Symbol(string.Empty, ReturnType.Bool, context.booleanExpression().Accept(this).AsBool && context.logicalExpression(0).Accept(this).AsBool);
          case "||": return new Symbol(string.Empty, ReturnType.Bool, context.booleanExpression().Accept(this).AsBool || context.logicalExpression(0).Accept(this).AsBool);
        }
      }
      else
      {
        seleniumLogger.Log($"Visiting boolean expression with operator", SeleniumScriptLogLevel.InterpreterDetails);

        return VisitBooleanExpression(context.booleanExpression());
      }

      throw new SeleniumScriptVisitorException("Logical expression could not be parsed");
    }

    public override Symbol VisitBooleanExpression([NotNull] BooleanExpressionContext context)
    {
      var first = context.left.Accept(this);
      var second = context.right.Accept(this);
      var logicalOperator = context.booleanOperator().GetText();

      seleniumLogger.Log($"Evaluating {first} {logicalOperator} {second}", SeleniumScriptLogLevel.InterpreterDetails);

      switch (logicalOperator)
      {
        case "==": return new Symbol(string.Empty, ReturnType.Bool, first.AsString.Equals(second.AsString));
        case "!=": return new Symbol(string.Empty, ReturnType.Bool, !first.AsString.Equals(second.AsString));
        case ">": return new Symbol(string.Empty, ReturnType.Bool, first.AsInt > second.AsInt);
        case "<": return new Symbol(string.Empty, ReturnType.Bool, first.AsInt < second.AsInt);
        case ">=": return new Symbol(string.Empty, ReturnType.Bool, first.AsInt >= second.AsInt);
        case "<=": return new Symbol(string.Empty, ReturnType.Bool, first.AsInt <= second.AsInt);
      }

      throw new SeleniumScriptVisitorException("Logical operator could not be parsed");
    }

  }
}
