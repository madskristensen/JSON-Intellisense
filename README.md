Package Intellisense for Visual Studio
=================

[![Build status](https://ci.appveyor.com/api/projects/status/p4c2fevy6oyd2eoa)](https://ci.appveyor.com/project/madskristensen/json-intellisense)

NPM and Bower package Intellisense directly in the Visual Studio JSON editor.

__Note__ This extension requires VS2013 Update 3 RC or newer.  

Download this extension from the [VS Gallery](http://visualstudiogallery.msdn.microsoft.com/65748cdb-4087-497e-a394-2e3449c8e61e)  
or get the [nightly build](http://vsixgallery.com/extension/aaa8d5c5-24d8-4c45-9620-9f77b2aa6363/).

## Features

### Intellisense  
Live search results from both NPM and Bower show up in Intellisense:  

![Screenshot](https://raw.githubusercontent.com/madskristensen/JSON-Intellisense/master/art/completion-name.png)

It can even show the package version as well:

![Screenshot](https://raw.githubusercontent.com/madskristensen/JSON-Intellisense/master/art/completion-version.png)

Please notice that npm version Intellisense is much faster than Bower's. 
This is due to Bower's slower API.


### Hover tooltips  
Get the information about the packages on mouse hover

Light theme:  
![Screenshot](https://raw.githubusercontent.com/madskristensen/JSON-Intellisense/master/art/tooltip-light.png)

Dark theme:  
![Screenshot](https://raw.githubusercontent.com/madskristensen/JSON-Intellisense/master/art/tooltip-dark.png)

Animated:   
![Screenshot](https://raw.githubusercontent.com/madskristensen/JSON-Intellisense/master/art/tooltip-animated.gif)
  

### Smart Tags  
Smart Tags are small helpers that offers contextual features

![Screenshot](https://raw.githubusercontent.com/madskristensen/JSON-Intellisense/master/art/smart-tags.png)

### Editor watermarks
Sometimes JSON files tend to look the same, so watermarks are
added to the bottom right corner to identify what file you are working in.

![Screenshot](https://raw.githubusercontent.com/madskristensen/JSON-Intellisense/master/art/watermark.png)

The watermark logos are added to:

* bower.json
* package.json
* gruntfile.js
* gulpfile.js

### Package restore on solution open
Make sure you always have both npm and Bower packages restored when you
open a project in Visual Studio. 

**Tools -> Options -> Package Intellisense** let's you modify the behavior
of this feature.

### Package restore on save
Any time you save either package.json or bower.json the packages will be 
restored/installed automatically in the background.