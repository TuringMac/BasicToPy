grammar Basic;

/*
 * Parser Rules
 */

line            : NUMBER statement CR
                | statement CR
                ;
statement       : PRINT exprlist
                | IF expression RELOP expression THEN statement
                | GOTO expression
                | INPUT varlist
                | LET VAR ASSIGN expression
                | GOSUB expression
                | RETURN
                | CLEAR
                | LIST
                | RUN
                | END
                ;

/*
 * Lexer Rules
 */

DIGIT           : [0-9] ;
VAR             : [A-Z] ;
NUMBER          : DIGIT DIGIT* ;
PLUS            : '+' ;
MINUS           : '-' ;
STAR            : '*' ;
SLASH           : '/' ;
ASSIGN          : '=' ;
GTE             : '>' ;
LTE             : '<' ;
RELOP           : LTE (GTE|ASSIGN)*
                | GTE (LTE|ASSIGN)*
                | ASSIGN
                ;
COMMA           : ',' ;
exprlist        : (string | expression) (COMMA (string | expression) )* ;
varlist         : VAR (COMMA VAR)* ;
expression      : (PLUS | MINUS) term ((PLUS | MINUS) term)* ;
term            : factor ((STAR | SLASH) factor)* ;
factor          : VAR | NUMBER | (expression) ;
string          : ' ( | ! | # | $ | - | . | / | DIGIT | : | @ | A | B | C | X | Y | Z )* ' ;
PRINT           : 'PRINT' ;
IF              : 'IF' ;
THEN            : 'THEN' ;
GOTO            : 'GOTO' ;
INPUT           : 'INPUT' ;
LET             : 'LET' ;
GOSUB           : 'GOSUB' ;
RETURN          : 'RETURN' ;
CLEAR           : 'CLEAR' ;
LIST            : 'LIST' ;
RUN             : 'RUN' ;
END             : 'END' ;
CR              : '\r\n' ;
