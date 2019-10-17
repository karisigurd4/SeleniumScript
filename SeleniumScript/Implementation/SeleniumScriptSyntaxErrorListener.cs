namespace SeleniumScript.Implementation
{
  using Antlr4.Runtime;
  using Antlr4.Runtime.Misc;
  using global::SeleniumScript.Interfaces;
  using System;

  public class SeleniumScriptSyntaxErrorListener : BaseErrorListener
  {
    private readonly ISeleniumScriptLogger seleniumScriptLogger;

    public SeleniumScriptSyntaxErrorListener(ISeleniumScriptLogger seleniumScriptLogger)
    {
      this.seleniumScriptLogger = seleniumScriptLogger;
    }

    public override void SyntaxError([NotNull] IRecognizer recognizer, [Nullable] IToken offendingSymbol, int line, int charPositionInLine, [NotNull] string msg, [Nullable] RecognitionException e)
    {
      seleniumScriptLogger.Log($"Line: {line}, Char: {charPositionInLine} on value {offendingSymbol.Text}: {msg}", Enums.LogLevel.SyntaxError);
      base.SyntaxError(recognizer, offendingSymbol, line, charPositionInLine, msg, e);
    }
  }
}
