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
    public override Symbol VisitUnaryExpression([NotNull] UnaryExpressionContext context)
    {
      var identifier = context.IDENTIFIER().GetText();
      var value = callStack.ResolveVariable(identifier).AsInt;
      var unaryOperator = context.unaryOperator().GetText();

      seleniumLogger.Log($"Applying unary operator {unaryOperator} to {identifier}", SeleniumScriptLogLevel.InterpreterDetails);

      switch (unaryOperator)
      {
        case "++": callStack.SetVariable(identifier, (value + 1).ToString()); break;
        case "--": callStack.SetVariable(identifier, (value - 1).ToString()); break;
      }

      return new Symbol(string.Empty, ReturnType.Int, callStack.ResolveVariable(identifier).AsInt);
    }

    public override Symbol VisitArithmeticExpression([NotNull] ArithmeticExpressionContext context)
    {
      ArithmeticExpressionContext findValue = context;
      if(context.left != null)
        while (findValue.value == null) findValue = findValue.left;
      else if (context.right != null)
        while (findValue.value == null) findValue = findValue.right;

      switch (findValue.value.Accept(this).ReturnType)
      {
        case ReturnType.Int: return HandleNumericalArithmetic(context);
        case ReturnType.String: return HandleStringArithmetic(context);
        default: throw new SeleniumScriptVisitorException($"Incorrect operation, cannot perform arithmetic on {context.left.Accept(this).ReturnType}");
      }
    }

    private Symbol HandleNumericalArithmetic(ArithmeticExpressionContext context)
    {
      Symbol left = null, right = null;

      if (context.value != null)
      {
        return new Symbol(string.Empty, ReturnType.Int, context.value.Accept(this).AsString);
      }

      if (context.left != null)
      {
        left = new Symbol(string.Empty, ReturnType.Int, HandleNumericalArithmetic(context.left).AsString);
      }

      if (context.right != null)
      {
        right = new Symbol(string.Empty, ReturnType.Int, HandleNumericalArithmetic(context.right).AsString);
      }

      string op = context.children[1].GetText();

      switch (op)
      {
        case "+": return new Symbol(string.Empty, ReturnType.Int, left.AsInt + right.AsInt);
        case "-": return new Symbol(string.Empty, ReturnType.Int, left.AsInt - right.AsInt);
        case "*": return new Symbol(string.Empty, ReturnType.Int, left.AsInt * right.AsInt);
        case "/": return new Symbol(string.Empty, ReturnType.Int, left.AsInt / right.AsInt);
        case "^": return new Symbol(string.Empty, ReturnType.Int, left.AsInt ^ right.AsInt);
        case "%": return new Symbol(string.Empty, ReturnType.Int, left.AsInt % right.AsInt);
      }

      throw new SeleniumScriptVisitorException("Invalid arithmetic operation");
    }

    private Symbol HandleStringArithmetic(ArithmeticExpressionContext context)
    {
      Symbol left = null, right = null;

      if (context.value != null)
      {
        return new Symbol(string.Empty, ReturnType.String, context.value.Accept(this).AsString);
      }

      if (context.left != null)
      {
        left = new Symbol(string.Empty, ReturnType.String, HandleStringArithmetic(context.left).AsString);
      }

      if (context.right != null)
      {
        right = new Symbol(string.Empty, ReturnType.String, HandleStringArithmetic(context.right).AsString);
      }

      string op = context.children[1].GetText();

      switch (op)
      {
        case "+": return new Symbol(string.Empty, ReturnType.String, left.AsString + right.AsString);
      }

      throw new SeleniumScriptVisitorException("Invalid operation");
    }
  }
}
