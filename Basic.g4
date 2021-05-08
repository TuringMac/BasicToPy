grammar Basic;

/*
 * Parser Rules
 */

line            : number statement CR
                | statement CR
                ;
statement       : PRINT exprlist                                    # stPrintExprList
                | IF expression relop expression THEN statement     # stIfThen
                | GOTO expression                                   # stGotoExpr
                | INPUT varlist                                     # stInputVarlist
                | LET VAR ASSIGN expression                         # stLetVarAssign
                | GOSUB expression                                  # stGosubExpr
                | RETURN                                            # stReturn
                | CLEAR                                             # stClear
                | LIST                                              # stList
                | RUN                                               # stRun
                | END                                               # stEnd
                ;

/*
 * Lexer Rules
 */

DIGIT           : [0-9] ;
CHAR            : [a-z] | [A-Z] | '.' ;
VAR             : [A-Z] ;
number          : DIGIT DIGIT* ;
LPAREN          : '(' ;
RPAREN          : ')' ;
PLUS            : '+' ;
MINUS           : '-' ;
STAR            : '*' ;
SLASH           : '/' ;
ASSIGN          : '=' ;
GTE             : '>' ;
LTE             : '<' ;
QUOTE           : '"' ;
relop           : LTE (GTE|ASSIGN)?
                | GTE (LTE|ASSIGN)?
                | ASSIGN
                ;
COMMA           : ',' ;
exprlist        : (string | expression) (COMMA (string | expression) )* ;
varlist         : VAR (COMMA VAR)* ;
expression      : (PLUS | MINUS)? term ((PLUS | MINUS) term)* ;
term            : factor ((STAR | SLASH) factor)* ;
factor          : VAR                                               # facVar
                | number                                            # facNumber
                | LPAREN expression RPAREN                          # facExpr
                ;
string          : '"' CHAR* '"' ;
PRINT           : 'PRINT' ;
IF              : 'If' ;
THEN            : 'Then' ;
GOTO            : 'GOTO' ;
INPUT           : 'INPUT' ;
LET             : 'Let' ;
GOSUB           : 'GOSUB' ;
RETURN          : 'RETURN' ;
CLEAR           : 'CLEAR' ;
LIST            : 'LIST' ;
RUN             : 'RUN' ;
END             : 'END' ;
CR              : '\r\n' ;
WS              : [ ]+ -> skip ;
