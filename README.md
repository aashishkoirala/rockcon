### RockCon: A Console-Based Music Player for Windows
##### By [Aashish Koirala](http://aashishkoirala.github.io)

I built this because I got sick of Windows Media Player modifying my music files without my consent, and also because I wanted to organize and get at my music just the way I like it - and through a clean console interface that is fast and easy to "keyboard around".  

Ironically, though, I am using the Windows Media Player COM control to actually play the music. I also use the open source NuGet package [ID3.NET](http://www.nuget.org/packages/ID3) to read/write ID3 tags.

The application can scan and construct its own library that is a simple to use JSON file. It then uses this to let you browse the library and play music. You can also synchronize your library information and your ID3 tags once you have your library information just the way you like it.

More to come - hopefully, but this will do for now.