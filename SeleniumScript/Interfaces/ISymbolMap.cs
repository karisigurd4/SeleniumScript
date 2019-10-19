namespace SeleniumScript.Interfaces
{
  public interface ISymbolMap
  {
    ISymbolScope CurrentScope { get; }
    void PushScope();
    void PopScope();
  }
}
