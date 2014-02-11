# ZephSharp

### About ZephSharp

(I'm terrible at naming projects, sorry.)

The Windows window manager for hackers

* Current version: 0.1
* Requires: Windows (not sure which versino) with .NET 4.5 (I think?)

#### Install

Haven't figured that part out yet.

#### Basics

At it's core, Zephyros just runs quietly in your systray (is it still called that?), and listens for your script.

You typically write a script that binds global hot keys to do stuff, like moving or resizing windows.

#### Stuff you can do

- register for callbacks on global hotkeys
- find the focused window
- determine window sizes and positions
- move and resize windows
- get free pizza (okay not really)
- more coming soon!

#### How you do it

You write your scripts in Clojure.

Put this in ~\AppData\Local\Zephyros\config.clj

```clojure
(bind "d" [:ctrl :alt :win]
      (fn []
        (-> (get-active-window)
            (move-window (get-screen-rect)))))
```

#### Change log

- 0.1
    - First working version

#### License

> Released under MIT license.
>
> Copyright (c) 2013 Steven Degutis
>
> Permission is hereby granted, free of charge, to any person obtaining a copy
> of this software and associated documentation files (the "Software"), to deal
> in the Software without restriction, including without limitation the rights
> to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
> copies of the Software, and to permit persons to whom the Software is
> furnished to do so, subject to the following conditions:
>
> The above copyright notice and this permission notice shall be included in
> all copies or substantial portions of the Software.
>
> THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
> IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
> FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
> AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
> LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
> OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
> THE SOFTWARE.
