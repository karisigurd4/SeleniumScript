﻿namespace SeleniumScript.Interfaces
{
  using static SeleniumScript.Grammar.SeleniumScriptParser;

  public interface ISeleniumScriptInterpreter
  {
    event CallBackEventHandler OnCallback;
    void Run(ExecutionUnitContext context);
  }
}
