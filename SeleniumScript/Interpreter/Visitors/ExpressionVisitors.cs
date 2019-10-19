namespace SeleniumScript.Implementation
{
  using Antlr4.Runtime.Misc;
  using global::SeleniumScript.Enums;
  using global::SeleniumScript.Exceptions;
  using global::SeleniumScript.Grammar;
  using static global::SeleniumScript.Grammar.SeleniumScriptParser;

  public partial class SeleniumScriptInterpreter : SeleniumScriptBaseVisitor<object>
  {
    public override object VisitLogicalExpression([NotNull] LogicalExpressionContext context)
    {
      if (context.logicalExpression().Length > 0 && context.logicalExpression(0) != null)
      {
        var logicalOperator = context.logicalOperator(0).GetText();

        seleniumLogger.Log($"Visiting logical expression with operator {logicalOperator}", SeleniumScriptLogLevel.InterpreterDetails);

        switch (logicalOperator)
        {
          case "&&": return (bool)context.booleanExpression().Accept(this) && (bool)context.logicalExpression(0).Accept(this);
          case "||": return (bool)context.booleanExpression().Accept(this) || (bool)context.logicalExpression(0).Accept(this);
        }
      }
      else
      {
        seleniumLogger.Log($"Visiting boolean expression with operator", SeleniumScriptLogLevel.InterpreterDetails);

        return VisitBooleanExpression(context.booleanExpression());
      }

      throw new SeleniumScriptVisitorException("Logical expression could not be parsed");
    }

    public override object VisitBooleanExpression([NotNull] BooleanExpressionContext context)
    {
      var first = context.data()[0].Accept(this);
      var second = context.data()[1].Accept(this);
      var logicalOperator = context.booleanOperator().GetText();

      seleniumLogger.Log($"Evaluating {first} {logicalOperator} {second}", SeleniumScriptLogLevel.InterpreterDetails);

      switch (logicalOperator)
      {
        case "==": return first.Equals(second);
        case "!=": return !first.Equals(second);
        case ">": return int.Parse((string)first) > int.Parse((string)second);
        case "<": return int.Parse((string)first) < int.Parse((string)second);
        case ">=": return int.Parse((string)first) >= int.Parse((string)second);
        case "<=": return int.Parse((string)first) <= int.Parse((string)second);
      }

      throw new SeleniumScriptVisitorException("Logical operator could not be parsed");
    }

    public override object VisitUnaryExpression([NotNull] UnaryExpressionContext context)
    {
      var identifier = context.IDENTIFIER().GetText();
      var value = (int)int.Parse(callStack.ResolveVariable(identifier));
      var unaryOperator = context.unaryOperator().GetText();

      seleniumLogger.Log($"Applying unary operator {unaryOperator} to {identifier}", SeleniumScriptLogLevel.InterpreterDetails);

      switch (unaryOperator)
      {
        case "++": callStack.SetVariable(identifier, (value + 1).ToString()); break;
        case "--": callStack.SetVariable(identifier, (value - 1).ToString()); break;
      }

      return (int)int.Parse(callStack.ResolveVariable(identifier));
    }

  }
}
