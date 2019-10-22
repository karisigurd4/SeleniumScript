# SeleniumScript

A scripting language and interpreter that provides a runtime and an interface for Selenium WebDriver. Intended to make the process of writing frontend tests quicker and easier to maintain. Any flavour of IWebDriver can be plugged into the component. The language has similar syntax to C for what it supports. The code is parsed and run by an interpreter and does not require compilation. 

The language is defined with ANTLR and the interpreter is implemented in C#.

Still under development, planned additions include
* VS Code integration 
* PreParser component (type checking, include scripts, etc...) 
* Test runner application 

## Usage (C# Component initialization)

The interpreter component exposes a run method which takes in a script as a string. The component implements IDisposable which releases and quits the executing selenium runtime when invoked. 

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
* **SendKeys**(*xPath*, *data*, (optional) *description*); - *Sends virtual key inputs to a web element via Selenium*
* **Click**(*xPath*, (optional) *description*); - *Performs a virtual click on a web element via selenium*
* **GetElementText**(*xPath*, (optional) *description*) - *Returns inner text contained within a web element via Slenium*
* **GetUrl**() - *Gets the current URL the webdriver is on*
* **NavigateTo**(*url*); - *Navigates the driver to a new URL*
* **Wait**(*numberOfSeconds*); - *Waits a specified number of seconds*
* **Log**(*message*) - *Writes to the **Script** log output*
* **Random**(*min/max*, (optional) *max*); - *Generates a random number between min and max. If only one parameter is specified min is trated as max.*
* **Callback**(*callbackName*); - *Triggers a callback to any of the registered callback handlers.*

SeleniumScript automatically creates log output on the SeleniumInfo log level for each of these operations. When the optional description parameters are used it serves to provide more comprehensive and usable diagnostic and monitoring information. 

### Variables
The following variable types are currently supported
* **int**
* **string**

Examples
```C#
string variableName = "Hello world!";
variableName = "New value!";
string currentUrl = GetUrl();
string elementText = GetElementText("//h1[@id = 'DynamicHeading_productTitle']");
int numberOfSeconds = 3;
Wait(numberOfSeconds);
```  

### Functions

Functions can be defined to execute a sequence of instructions with an optional return type and optional parameter list declarations. Currently functions need to be specified at the top of the file for the interpreter to analyze/register them and recognize later uses of them.

**Simple function definition**
```C#
MyFunc() 
{ 
  Log("This is a function"); 
} 
```

**Parameterized function with a return statement**
```C#
string GetTopResultOnGoogle(string searchString)
{
  NavigateTo("https://www.google.com/");
  SendKeys("//input[@name = 'q']", searchString, ""Search bar"");
  Click("//input[@name = 'q']//following::a", searchString, ""Search bar"");
  
  return GetElementText("//div[@class = 'g']", "First search result");
}
```  

### Control flow

**If - else if - else statements**
```C#
if ("same" == "notsame") 
{ 
  Wait(10); 
} 
else if ("same"" == "same") 
{ 
  return "hello world!" 
} 
else 
{ 
  NavigateTo("Url"); 
}
```  

**For loops**
```C#
for (int i = 0; i < 10; i++)
{
  Log(i);
}
```   

**Arithmetic**

Numerical arithmetic follows operator precedence

```C#
int i;
i = 3 + 2 * 2;
```   

String arithmetic

```C#
string s;
s = "Hello" + " World!";
```   


### Type handling

It is a dynamically typed language but strong type safety is enforced during runtime, e.g., you cannot assign a string to an int. However the built in operations accept any type.

### Scope handling

The language has global, local and method scope handling. Any execution of statements contained within curly braces creates an implicit local scope. The global scope is accessible by any part of the script at any time but method scopes are invisible to other method scopes and any local scopes contained within them. 

## Callbacks

You can define any number of custom callback handlers that you can then reference in your scripts. This is useful in situations where you need to let the script trigger an event in your backend. Your registered callback handler can route operations accordingly.

```C#
string script = "Callback(\"callback\");";

string callbackOutput = string.Empty;

using (var seleniumScript = new SeleniumScript(new ChromeDriver(new ChromeOptions() { LeaveBrowserRunning = false })))
{
  seleniumScript.RegisterCallbackHandler("callback", () => { callbackOutput = "Assigned"; });
  seleniumScript.Run(script);
}
```  

## Logging

SeleniumScript has the following levels of logging,
* **Script -** Output generated by script **Log** function calls
* **VisitorDetails -** Diagnostic output from the visitors
* **SeleniumInfo -** Diagnosic output from the Selenium WebDriver abstraction in SeleniumScript
* **SyntaxError -** Syntactic error logs
* **VisitorError -** Visitor errors
* **WebDriverError -** Errors occurring in the Selenium WebDriver abstraction
* **SeleniumError -** Errors occurring in the top level SeleniumScript component
* **RuntimeError -** Errors that might occur in the overlying C# runtime

### Log routing

It is up to the client using SeleniumScript to hook up the **OnLogEntryWritten** event handler and route the logs as they please.

```C#
seleniumScript.OnLogEntryWritten += (log) =>
{
  Debug.WriteLine(log.Message);
};
```

### SeleniumInfo logs

The SeleniumInfo log level will provide an overview of the steps taken in your scripts. This is especially useful for cases like test logs. Below is a formatted example of what the output from a script might look like.

    2019-10-17 21:35:20 [SeleniumInfo] - Navigating driver to https://www.microsoft.com/sv-se/
    2019-10-17 21:35:21 [SeleniumInfo] - Clicking element search button
    2019-10-17 21:35:21 [SeleniumInfo] - Sending keys Windows 10 to search input
    2019-10-17 21:35:22 [SeleniumInfo] - Clicking element First search result
    2019-10-17 21:35:22 [SeleniumInfo] - Operation not successful, retrying...
    2019-10-17 21:35:25 [SeleniumInfo] - Sending keys fakeemail@fakeemail to email address input
    2019-10-17 21:35:26 [SeleniumInfo] - Waiting for 1 seconds
    2019-10-17 21:35:27 [SeleniumInfo] - Clicking element Close dialog button
    2019-10-17 21:35:27 [SeleniumInfo] - Finding text on element Product title
    2019-10-17 21:35:27 [SeleniumInfo] - Navigating driver to https://www.google.com/
    2019-10-17 21:35:27 [SeleniumInfo] - Sending keys Windows 10 Home to Search bar
    2019-10-17 21:35:27 [SeleniumInfo] - Closing and quitting webdriver
