Recent Updates
==========
RunExp now waits for the previous mission to end before attempting to start a new one.

Previously it would attempt to start a new one anyway, but fail if there is an existing mission.

It will also end the mission and refuel the ships, making the transition seamless.

Additionally, the program will abort the loop if any mission fails.

Getting the algorithm to generate api_port is getting harder and harder.
I fear that I may not be able to get it sooner. Relying on Japanese BBS to get the algorithm now.

Overview
==========
This solution contains a few things:

KanColleAPI
==========
A library written in C# that extracts and interprets JSON player information from the KanColle server.

This is mainly written as an exercise in C# for me. I take no responsibility if you use this library for any hanky-panky and get banned.

Always a work in progress.

Build this to get the DLL.

This library provides a KanColleProxy class that sends and receives a POST request. The current user-agent is Firefox.

The KanColleProxy class outputs a raw JSON string which you must then implement methods to deal with yourself. Personally I used JSON.NET to achieve this.

RunExp
==========
Probably what you are here for.

This is a simple command line program that loops expedition commands.

Sparkler
==========
Another simple cmd line program that sends a single ship out on 1-1 sparkle battles.
You specify which fleet to send out and how many times to sparkle.
This program will tell you the morale/condition of the ship every loop.

[UPDATE] Now this program will skip iterations if the morale is 81 or greater.

To-do
========
Add stuff that interprets battle data.

Add user agent extensions.
