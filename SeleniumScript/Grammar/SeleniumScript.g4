grammar SeleniumScript;

/*
 * Parser Rules
 */

 executionUnit: statement* EOF;

 statement
	: ifCondition
	| operationSendkeys
	| operationNavigateTo
	| operationClick
	| operationWait
	| operationLog
	| variableDeclaration
	| variableAssignment
	;

 statementBlock: '{' statement* '}';

 operationLog: LOG parameterList EOL;
 operationSendkeys: SENDKEYS parameterList EOL;
 operationClick: CLICK parameterList EOL;
 operationNavigateTo: NAVIGATETO parameterList EOL;
 operationWait: WAIT parameterList EOL;
 operationGetElementText: GETELEMENTTEXT parameterList;
 operationGetUrl: GETURL parameterList;

 parameterList: '(' (data COMMA?)* ')';

 ifCondition
	: IF '(' comparison ')' statementBlock (ELSE ifCondition)* (ELSE statementBlock)?
	;

 comparison
	:  data COMPARISON data 
	| (data COMPARISON comparison)*
	;

 variableDeclaration: variableType variableAssignment;
 variableAssignment: IDENTIFIER ASSIGNMENT data EOL;

 data
	: resolveReference
	| resolveLiteral
	| operationGetElementText
	| operationGetUrl
	;

 resolveReference: IDENTIFIER;
 resolveLiteral
	: resolveStringLiteral
	| resolveIntLiteral
	;

resolveIntLiteral: INTLITERAL; 
resolveStringLiteral: STRINGLITERAL;

 variableType: STRING;

/*
 * Lexer Rules
 */

 IF: 'if';
 ELSE: 'else';

 STRING: 'string';
 INT: 'int';
 DOUBLE: 'double';

 LOG: 'Log';
 SENDKEYS: 'SendKeys';
 CLICK: 'Click';
 WAIT: 'Wait';
 NAVIGATETO: 'NavigateTo';

 GETELEMENTTEXT: 'GetElementText';
 GETURL: 'GetUrl';

 STRINGLITERAL: DOUBLEQUOTE .*? DOUBLEQUOTE;
 INTLITERAL: NUMBER+;
 DOUBLELITERAL: NUMBER+ DOT NUMBER*;

 IDENTIFIER: [A-Za-z]+;

 COMPARISON: '==';
 ASSIGNMENT: '=';
 EOL: ';';
 COMMA: ',';

 WHITESPACE: WS -> skip;

 fragment DOT: '.';
 fragment DOUBLEQUOTE: '"';
 fragment NUMBER: [0-9];
 fragment WS: [ \t\r\n]+;