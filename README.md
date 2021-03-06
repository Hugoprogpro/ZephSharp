# ZephSharp

### About ZephSharp

(I'm terrible at naming projects, sorry.)

The Windows window manager for hackers

* Current version: 0.1
* Requires: Windows (not sure which version) with .NET 4.5 (I think?)

#### Install

- Download [Zephyros-0.1.zip](https://github.com/sdegutis/ZephSharp/blob/master/Builds/Zephyros-0.1.zip?raw=true), unzip, run setup.exe, click "More Info", click "Run Anyway" (uses "ClickOnce").

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

Right now you can only write your scripts in Clojure. There are plans to use other languages in the future, but I've yet to figure out which ones people want integrated in.

Put this in ~\AppData\Local\Zephyros\config.clj

```clojure
(bind "d" [:ctrl :alt :win]
      (fn []
        (-> (get-active-window)
            (move-window (get-screen-rect)))))
```

Then start ZephSharp. It will run your script. Click the icon in the systray to reload your script.

Here's [the rest the API](https://github.com/sdegutis/ZephSharp/blob/master/Zephyros/Setup.clj). Also you can use `:shift` as a modifier too.

#### Todo

- Get a better systray icon (just borrowed some random favicon.ico for now).
- Better error messages when your script doesn't load so well.

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
