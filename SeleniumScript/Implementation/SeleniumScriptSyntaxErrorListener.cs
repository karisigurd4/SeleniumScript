namespace SeleniumScript.Implementation
{
  using Antlr4.Runtime;
  using Antlr4.Runtime.Misc;
  using global::SeleniumScript.Exceptions;
  using global::SeleniumScript.Interfaces;
  using System;

  public class SeleniumScriptSyntaxErrorListener : BaseErrorListener
  {
    public override void SyntaxError([NotNull] IRecognizer recognizer, [Nullable] IToken offendingSymbol, int line, int charPositionInLine, [NotNull] string msg, [Nullable] RecognitionException e)
    {
      throw new SeleniumScriptSyntaxException($"Line: {line}, Char: {charPositionInLine} on value {offendingSymbol.Text}: {msg}");
    }
  }
}
