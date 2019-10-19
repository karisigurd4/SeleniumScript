namespace SeleniumScript.Implementation
{
  using global::SeleniumScript.Interfaces;
  using System.Collections.Generic;

  public class SymbolMap : ISymbolMap
  {
    private Stack<SymbolScope> scopedSymbols;

    public ISymbolScope CurrentScope => scopedSymbols.Peek();

    public SymbolMap()
    {
      scopedSymbols.Push(new SymbolScope(null));
    }

    public void PushScope()
    {
      scopedSymbols.Push(new SymbolScope(scopedSymbols.Peek()));
    }

    public void PopScope()
    {
      scopedSymbols.Pop();
    }
  }
}
