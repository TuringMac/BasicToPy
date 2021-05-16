/*
BSD License
Copyright (c) 2017, Tom Everett
All rights reserved.
Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions
are met:
1. Redistributions of source code must retain the above copyright
   notice, this list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright
   notice, this list of conditions and the following disclaimer in the
   documentation and/or other materials provided with the distribution.
3. Neither the name of Tom Everett nor the names of its contributors
   may be used to endorse or promote products derived from this software
   without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

grammar Basic;

program
    : line* EOF
    ;

line
   : number statement CR
   | statement CR
   ;

statement
   : 'PRINT' exprlist                                       # stPrintExprList
   | 'IF' expression relop expression 'THEN' statement      # stIfThen
   | 'GOTO' number                                          # stGotoExpr
   | 'INPUT' varlist                                        # stInputVarlist
   | 'LET' VAR '=' expression                               # stLetVarAssign
   | 'GOSUB' expression                                     # stGosubExpr
   | 'RETURN'                                               # stReturn
   | 'CLEAR'                                                # stClear
   | 'LIST'                                                 # stList
   | 'RUN'                                                  # stRun
   | 'END'                                                  # stEnd
   | 'REM' STRING                                           # stRem
   | 'RND' VAR                                              # stRnd
   | 'DIM' VAR 'AS' TYPE                                    # stDim
   ;

exprlist
   : (STRING | expression)
   | exprlist ',' (STRING | expression)
   ;

varlist
   : VAR
   | varlist ',' VAR
   ;

expression
   : sign=(PLUS | MINUS )? term                             # expSingle
   | expression (op=(PLUS | MINUS) term)                    # expDuo
   ;

term
   : factor                                                 # termSingle
   | term (op=(STAR | SLASH) factor)                        # termDuo   
   ;

factor
   : VAR                                                    # facVar
   | number                                                 # facNumber
   | '(' expression ')'                                     # facExpr
   ;

number
   : DIGIT +
   ;

relop
   : ('<' ('>' | '=' )?)
   | ('>' ('<' | '=' )?)
   | '='
   ;

PLUS
   : '+'
   ;
MINUS
   : '-'
   ;
STAR
   : '*'
   ;
SLASH
   : '/'
   ;
TYPE
   : 'INTEGER'
   | 'LONG'
   | 'SINGLE'
   | 'DOUBLE'
   | 'STRING'
   ;
STRING
   : '"' ~ ["\r\n]* '"'
   ;
DIGIT
   : '0' .. '9'
   ;
VAR
   : 'A' .. 'Z'
   ;
CR
   : [\r\n]+
   ;
WS
   : [ \t] -> skip
   ;