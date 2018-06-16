# Hammer.MdiContainer
This is a fork from the excellent MDIContainer for WPF by András Sebő. You can find the original CodePlex project here:

https://mdicontainer.codeplex.com/

##Original Description
MDIContainer is a custom WPF control (also part of the Hammer UI Kit), allows you to display user controls as windows in a container. The motive was to make available to work with multiple documents in the same time.

The concept of MDI is not new, in fact it is depricated in WPF. Microsoft recommends to use TabControl, Ribbon or Dockable components. However sometimes it would be still better to have windows.

Features

    Displays any user control (even different kind of at the same time) as MDI Window
    MDI Window supports minimize, maximize, close, move and resize
    Implementation is as easy as a TabControl
    MDI Window has almost same behavior as a MS Window
    CTRL + TAB and CTRL + SHIFT + TAB swtiches between windows
    TAB and SHIFT + TAB switches between controls inside the window
    Show thumbnail image in minimized mode
    
    
##Modifications
I have made a few fixes/enhancements:
* Windows cannot be mistakenly hidden outside of the visible area of the container
* Keyboard focus outside of the container does not modify selected window in the container 
* Date picker popup causes the window to be active
* There is a new DependencyProperty: IsModal, which draws an adorner around the window so that other windows cannot take focus, making this window modal. Use only on the active window, otherwise you will experience something like a Venn's diagram effect :)
* Refactored a few names - simplified code here and there.


The source code is licensed under the GNU General Public License version 3 (GPLv3)

#Installation
This project will be uploaded to NuGet repositories: https://www.nuget.org/packages/MdiContainerWPF/

PM> Install-Package MdiContainerWPF 
