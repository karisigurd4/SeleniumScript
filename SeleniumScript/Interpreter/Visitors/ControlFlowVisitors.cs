namespace SeleniumScript.Implementation
{
  using Antlr4.Runtime.Misc;
  using global::SeleniumScript.Enums;
  using global::SeleniumScript.Grammar;
  using global::SeleniumScript.Implementation.DataModel;
  using global::SeleniumScript.Implementation.Enums;
  using global::SeleniumScript.Interpreter.Enums;
  using static global::SeleniumScript.Grammar.SeleniumScriptParser;

  public partial class SeleniumScriptInterpreter : SeleniumScriptBaseVisitor<Symbol>
  {
    public override Symbol VisitStatementBlock([NotNull] StatementBlockContext context)
    {
      callStack.Push(StackFrameScope.Local);
      base.VisitStatementBlock(context);
      callStack.Pop();
      return null;
    }

    public override Symbol VisitIfCondition([NotNull] IfConditionContext context)
    {
      if (context.logicalExpression().Accept(this).AsBool)
      {
        seleniumLogger.Log($"if condition held, visiting statment block", SeleniumScriptLogLevel.InterpreterDetails);

        callStack.Push(StackFrameScope.Local);
        context.statementBlock(0).Accept(this);
        callStack.Pop();

        return new Symbol(string.Empty, ReturnType.Bool, true);
      }
      else if (context.ifCondition().Length > 0 && context.ifCondition()[0] != null)
      {
        seleniumLogger.Log($"else if condition held, visiting statment block", SeleniumScriptLogLevel.InterpreterDetails);

        context.ifCondition(0).Accept(this);
      }
      else
      {
        if (context.ELSE() != null && context.ELSE().Length == 1 && context.statementBlock().Length == 2)
        {
          seleniumLogger.Log($"Conditions did not hold, contained else, visiting statment block", SeleniumScriptLogLevel.InterpreterDetails);

          callStack.Push(StackFrameScope.Local);
          context.statementBlock(1).Accept(this);
          callStack.Pop();

          return new Symbol(string.Empty, ReturnType.Bool, true);
        }
      }
      return null;
    }

    public override Symbol VisitForLoop([NotNull] ForLoopContext context)
    {
      var forLoopInitializer = context.forLoopArguments().forLoopInitializer();
      var booleanExpression = context.forLoopArguments().booleanExpression();
      var unaryExpression = context.forLoopArguments().unaryExpression();
      var statementBlock = context.statementBlock();

      forLoopInitializer.Accept(this);

      seleniumLogger.Log($"Entering for loop", SeleniumScriptLogLevel.InterpreterDetails);
      while (booleanExpression.Accept(this).AsBool)
      {
        seleniumLogger.Log($"Loop condition holds, exeucting statement block", SeleniumScriptLogLevel.InterpreterDetails);
        statementBlock.Accept(this);
        unaryExpression.Accept(this);
      }
      seleniumLogger.Log($"Exited for loop", SeleniumScriptLogLevel.InterpreterDetails);

      return null;
    }
  }
}
