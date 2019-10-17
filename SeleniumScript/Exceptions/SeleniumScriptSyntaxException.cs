namespace SeleniumScript.Exceptions
{
  using System;
 
  public class SeleniumScriptSyntaxException : Exception
  {
    public SeleniumScriptSyntaxException(string message) : base(message)
    {
    }
  }
}
