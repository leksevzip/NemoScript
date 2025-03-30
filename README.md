# NemoScript
Fully written on C# programming language (kind of). This project i made in spring break. Don't expect too much from this, it's been made just for fun.

> That language uses ```.nemo``` file extension and can only be used for console applications. It can't compile apps to .exe or any binary file.

## Documentation
Documentation is avaliable [here](../Documentation/NemoScript Documentation (Read in Notepad++).txt)

## Code Editor
You can use my NemoScript KIDE but i ***highly*** recommend to use [Notepad++](https://notepad-plus-plus.org/downloads/) with NS Syntax.

## Installation and getting started
Installation is actually pretty simple as downloading any other windows apps.
## Windows
1. Download the ```nemoscript-beta-win-x64.exe``` on your PC
2. Rename it to ```NemoScript.exe```
3. Open command line and invoke it from there
## Linux
1. Download the ```nemoscript-beta-linux-x64``` on your PC
2. Rename it to ```NemoScript```
3. Open terminal and invoke it from there

When invoking NemoScript you need to specify the script file.  
> ```NemoScript.exe [FileName].nemo```

## Code Examples?
Here's the simple program that i pronounce as "The Hello, World! Program"
```
say>
"The Hello World! Program"
<exit
say>
"Fully written on NemoScript"
<exit
say>
"Write any num: "
<exit
var>
num
thenum
waitinput
<exit
var>
num
i
0
<exit
var>
text
smth
""
<exit
while>
i
<
thenum
say>
"Write smth: "
<exit
smth>
waitinput
<smth
if>
smth
=
"Bye, World!"
say>
"Bye, world!:C"
<exit
endprog
<endif
say>
"Text: "
<exit
say>
smth
<exit
varcalc>
i
+
1
<exit
<endwhile
say>
"Bye, World!:C"
<exit
```
Other examples are [here](../Examples/)
