# Expressions

## Binary

Supported operators: `+, -, *, :`

```js
1 + 1; // 2
"str" + "ing"; // string

1 - 1; // 0
2 * 4; // 8
4 / 2; // 2
```

## Logical

Supported operators: `||, &&`

```js
true || false; // true
true && false; // false
```

## Unary

```js
-1; // -1
!true; // false
```

## Literal

```js
// Numbers
1;
2.3;

// True and false
true;
false;

// Strings
"Hello World!"

// Lists
[1, 2, 3];
```

## Grouped

```js
(2 + 2) * 4; // 16
```

## Ternary

```js
1 == 1 ? true : false; // If 1 == 1 then true else false (Expected: true) 
```

# Variables

Variables can have the following types: `number, string, bool, list`.

Number stores any double number, also signed.

Strings store text of any length.

Bools store state of either true or false.

Lists store literals of any type, including other lists.

```js
// Default initializers
var number a; // 0
var string b; // ""
var bool c; // false
var list d; // []

// Initialize
var number e = 2;

// Access
puts e;
```

# Functions

## Void functions

Void functions return void by default. You can't do much with it.

`return` in void functions statements return null.

```js
function void betterPrint(string text)
{
    print text;
}

betterPrint("Hello World!");
```

## Other

Non-void functions have to return a value of given type via `return`.

```js
function number betterAdd(number add1, number add2)
{
    return add1 + add2;
}

puts betterAdd(1, 2);
```

# Control flow

## If and Switch

```js

if(true)
{
    puts "Truth!";
} else if (false)
{
    puts "Unreachable";
}

// TODO Switch statements currently do not work and are under heavy works
switch(true)
{
    case true:
    {
        puts "Truth!";
        // Do note that unlike in other langs there's currently no pass through
        // and all switch cases break by default
    }
    
    case false:
    {
        puts "Unreachable!";
    }
    
    default:
    {
        puts "Also unreachable!";
    }
}
```

## Loops

### Finite

```js
var number i = 10;

while(i > 0)
{
    puts i;
    i = i - 1;
}

var number a = 0;
var number temp = 0;

for (var number b = 1; a < 10000; b = temp + b)
{
    puts a;
    temp = a;
    a = b;
}
```

### Infinite

You use `break` to break out of infinite loops.

```js
var number a = 1;
while(true)
{
    a = a + 1;
    
    if(a > 10)
    {
        break;
    }
}

for(;;)
{

}
```

# Lists

## Declaration

Lists can store a (theoretically) infinite number of all types of values.

```js
// TODO lists are currently in heavy development
[1, "hello", [2, 3], true, foo()]; // [1, "hello", [2, 3], true, true]

function bool foo()
{
    return true;
}
```

## Access

```js
var list a = [1, 2];
var list b = [[7, 8], 3];

puts a[0]; // 1

// Do note the unusual, reversed order of [1][0], 
// this had to be done because of how the interpreter/parser work, sorry!
puts b[1][0]; // 8
```