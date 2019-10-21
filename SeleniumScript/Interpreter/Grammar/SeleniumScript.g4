grammar SeleniumScript;

/*
 * Parser Rules
 */

 executionUnit
	: statement* EOF
	;

 statement
	: ifCondition
	| operationSendkeys
	| operationNavigateTo
	| operationClick
	| operationWait
	| operationLog
	| variableDeclaration
	| variableAssignment
	| functionDeclaration
	| functionCall
	| functionReturn
	| statementBlock
	| expression SEMI
	| forLoop
	| operationCallBack
	;

 statementBlock
	: LCBRACKET statement* RCBRACKET
	;

 operationCallBack
	: CALLBACK parameterList SEMI
	;

 operationLog
	: LOG parameterList SEMI
	;

 operationSendkeys
	 : SENDKEYS parameterList SEMI
	 ;

 operationClick
	 : CLICK parameterList SEMI
	 ;

 operationNavigateTo
	 : NAVIGATETO parameterList SEMI
	 ;

 operationWait
	 : WAIT parameterList SEMI
	 ;

 operationGetElementText
	 : GETELEMENTTEXT parameterList
	 ;

 operationGetUrl
	 : GETURL parameterList
	 ;

 operationRandom
	 : RANDOM LPAREN first=resolveIntLiteral (COMMA second=resolveIntLiteral)? RPAREN
	;

 parameterList
	: LPAREN (data COMMA?)* RPAREN
	;
 
 forLoop
	: FOR LPAREN forLoopArguments RPAREN statementBlock
	;

 forLoopArguments
	: ( forLoopInitializer? booleanExpression? SEMI unaryExpression? )
	;

 forLoopInitializer
	: ( ( variableDeclaration | variableAssignment ) | SEMI )
	;

 ifCondition
	: IF LPAREN logicalExpression RPAREN statementBlock (ELSE ifCondition)* (ELSE statementBlock)?
	;

 expression
	: booleanExpression
	| unaryExpression
	;

 logicalExpression
	: booleanExpression ( logicalOperator logicalExpression )*
	;

 logicalOperator
	: AND
	| OR
	;

 booleanExpression
	: left=data booleanOperator right=data
	;

 booleanOperator
	: EQ
	| NE
	| LGT
	| LT
	| LGTE
	| LTE
	;

 unaryExpression
	: IDENTIFIER unaryOperator
	;

 unaryOperator
	: INCREMENT
	| DRECREMENT
	;

 functionCall
	: IDENTIFIER LPAREN functionArguments? RPAREN SEMI?
	;

 functionArguments
	: data ( COMMA data )*
	;

 functionDeclaration
	: variableType? IDENTIFIER functionParameters functionBody
	;

 functionParameters
	:  LPAREN (( variableType IDENTIFIER ) ( COMMA variableType IDENTIFIER )*)? RPAREN
	;

 functionBody
	:  statementBlock
	;

 functionReturn
	: RETURN data SEMI
	;

 variableDeclaration
	: variableType IDENTIFIER ( ASSIGNMENT data )? SEMI
	;
 
 variableAssignment
	: IDENTIFIER ASSIGNMENT data SEMI
	;

 data
	: resolveSymbol
	| arithmeticExpression
	;
 
 arithmeticExpression
	: left=arithmeticExpression POW right=arithmeticExpression
	| left=arithmeticExpression MUL right=arithmeticExpression
	| left=arithmeticExpression DIV right=arithmeticExpression
	| left=arithmeticExpression ADD right=arithmeticExpression
	| left=arithmeticExpression SUB right=arithmeticExpression
	| left=arithmeticExpression MOD right=arithmeticExpression
	| value=resolveSymbol
	;

 arithmeticOperator
	: ADD
	| SUB
	| MUL
	| DIV
	| POW
	| MOD
	;

 resolveSymbol
	: resolveReference
	| resolveLiteral
	| functionCall
	| operationGetElementText
	| operationGetUrl
	| operationRandom
	;

 resolveReference
	: IDENTIFIER
	;
 
 resolveLiteral
	: resolveStringLiteral
	| resolveIntLiteral
	;

resolveIntLiteral
	: INTLITERAL
	;

resolveStringLiteral
	: STRINGLITERAL
	;

 variableType
	: STRING
	| INT
	;

/*
 * Lexer Rules
 */

 LOG: 'Log';
 SENDKEYS: 'SendKeys';
 CLICK: 'Click';
 WAIT: 'Wait';
 NAVIGATETO: 'NavigateTo';
 CALLBACK: 'Callback';
 RANDOM: 'Random';

 GETELEMENTTEXT: 'GetElementText';
 GETURL: 'GetUrl';

 RETURN: 'return';

 FOR: 'for';

 IF: 'if';
 ELSE: 'else';

 STRING: 'string';
 INT: 'int';
 DOUBLE: 'double';

 STRINGLITERAL: DOUBLEQUOTE .*? DOUBLEQUOTE;
 INTLITERAL: NUMBER+;
 DOUBLELITERAL: NUMBER+ DOT NUMBER*;

 IDENTIFIER: [A-Za-z]+;

 EQ: '==';
 NE: '!=';
 AND: '&&';
 OR: '||';
 LGT: '>';
 LT: '<';
 LGTE: '>=';
 LTE: '<=';

 ADD: '+';
 SUB: '-';
 MUL: '*';
 DIV: '/';
 POW: '^';
 MOD: '%';

 ASSIGNMENT: '=';

 INCREMENT: '++';
 DRECREMENT: '--';

 SEMI: ';';
 COMMA: ',';
 LPAREN: '(';
 RPAREN: ')';
 LCBRACKET: '{';
 RCBRACKET: '}';

 WHITESPACE: WS -> skip;

 /*
 * Fragments
 */

 fragment DOT: '.';
 fragment DOUBLEQUOTE: '"';
 fragment NUMBER: [0-9];
 fragment WS: [ \t\r\n]+;