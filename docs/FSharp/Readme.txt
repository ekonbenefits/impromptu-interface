impromptu-interface.fsharp http://code.google.com/p/impromptu-interface/

A complete FSharp dynamic operator implementation using the DLR.

Copyright 2011-2012 Ekon Benefits
Apache Licensed: http://www.apache.org/licenses/LICENSE-2.0

Author:
Jay Tuley jay+code@tuley.name

Usage:
    Just open ImpromptuInterface.FSharp module it will give you the dynamic operator implemenatino

    obj?prop //return property
    obj?prop <- value //set a property
    obj?method(1,2) //call a method
    !?obj (1,2) //dynamically invoke a delegate, object or fsharp function
