# SeleniumScript
A scripting language abstraction built on top of Selenium WebDriver. Intended to make the process of writing frontend tests quicker and easier to maintain. Any flavour of IWebDriver can be plugged into the component. 

## Example script
```C#
NavigateTo("https://www.microsoft.com/sv-se/");
Click("//button[@id = 'search']");
SendKeys("//input[@id = 'cli_shellHeaderSearchInput']", "Windows 10");
Click("//input[@id = 'cli_shellHeaderSearchInput']//following::a", "First search result");
Click("//div[@class = 'sfw-dialog']//following::div[@class = 'c-glyph glyph-cancel']", "Close dialog button");
string productTitle = GetElementText("//h1[@id = 'DynamicHeading_productTitle']", "Product title");

NavigateTo("https://www.google.com/");
SendKeys("//input[@name = 'q']", productTitle, "Search bar");
Click("//input[@name = 'q']"//following::a, "First search result");
```

## Usage
```C#
var script = "NavigateTo("https://www.microsoft.com/sv-se/");  Click("//button[@id = 'search']"); ...";

var driver = new ChromeDriver();

using (var seleniumScript = new SeleniumScript(driver))
{
  seleniumScript.Run(script);
}
```

## Language
SeleniumScript provides a number of built-in functions,
* **SendKeys**(*xPath*, *data*, (optional) *description*);
* **Click**(*xPath*, (optional) *description*);
* **GetElementText**(*xPath*, (optional) *description*)
* **GetUrl**()
* **NavigateTo**(*url*);
* **Wait**(*numberOfSeconds*);
* **Log**(*message*)

SeleniumScript automatically creates log output on the SeleniumInfo log level for each of these operations. When the optional description parameters are used it serves to provide more comprehensive and usable diagnostic and monitoring information. 

### Variables
Variables can be declared and assigned a constant string literal or assigned the value of a built-in method result

```C#
string variableName = "Hello world!";
variableName = "New value!";
string currentUrl = GetUrl();
string elementText = GetElementText("//h1[@id = 'DynamicHeading_productTitle']");
```  

## Logging
SeleniumScript has the following levels of logging,
* **Script -** Output from script **Log** function calls
* **VisitorDetails -** Diagnostic output from the visitors
* **SeleniumInfo -** Diagnosic output from the Selenium WebDriver abstraction in SeleniumScript
* **SyntaxError -** Syntactic error logs
* **VisitorError -** Visitor errors
* **SeleniumError -** Errors occurring in the Selenium WebDriver abstraction
* **RuntimeError -** Errors that might occur in the overlying C# runtime

### Log routing
By default SeleniumScript will not do anything at all with the logs it generates. It is up to the client using SeleniumScript to hook up the **OnLogEntryWritten** event handler and route the logs as they please.

```C#
seleniumScript.OnLogEntryWritten += (log) =>
{
  Debug.WriteLine(log.Message);
};
```
