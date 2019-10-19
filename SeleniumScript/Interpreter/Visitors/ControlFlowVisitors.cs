namespace SeleniumScript.Implementation
{
  using Antlr4.Runtime.Misc;
  using global::SeleniumScript.Enums;
  using global::SeleniumScript.Grammar;
  using global::SeleniumScript.Implementation.Enums;
  using static global::SeleniumScript.Grammar.SeleniumScriptParser;

  public partial class SeleniumScriptInterpreter : SeleniumScriptBaseVisitor<object>
  {
    public override object VisitStatementBlock([NotNull] StatementBlockContext context)
    {
      callStack.Push(StackFrameScope.Local);
      base.VisitStatementBlock(context);
      callStack.Pop();
      return null;
    }

    public override object VisitIfCondition([NotNull] IfConditionContext context)
    {
      if ((bool)context.logicalExpression().Accept(this))
      {
        seleniumLogger.Log($"if condition held, visiting statment block", SeleniumScriptLogLevel.InterpreterDetails);

        callStack.Push(StackFrameScope.Local);
        context.statementBlock(0).Accept(this);
        callStack.Pop();

        return true;
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

          return true;
        }
      }
      return null;
    }

    public override object VisitForLoop([NotNull] ForLoopContext context)
    {
      var forLoopInitializer = context.forLoopArguments().forLoopInitializer();
      var booleanExpression = context.forLoopArguments().booleanExpression();
      var unaryExpression = context.forLoopArguments().unaryExpression();
      var statementBlock = context.statementBlock();

      forLoopInitializer.Accept(this);

      seleniumLogger.Log($"Entering for loop", SeleniumScriptLogLevel.InterpreterDetails);
      while ((bool)booleanExpression.Accept(this))
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
