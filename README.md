# PMS application for the Hellenic Navy

This application was created during my service in the Hellenic Navy (November 2022 - August 2023). 
It's a PMS (Programmed Maintenance Schedule) application for the machinery of  the ship I was serving at.

Since there was no internet access when I was working on this, the framework of choice was Unity because that's what I had installed on my laptop at the time.
Unity is used for all the UI infrastructure and timed events (Start, Update, Coroutines etc).

The application uses a local SQLite database to store and check all information about the machinery and when maintenance should be done, as well as all the necessary work that should be done. 
The user must input all the maintenance work that should be done (all tasks).
The application can also export a report of all the work that was completed for each task.

The communication between the application and the SQLite database is handled by a custom multi-threaded solution I made a few years before this.
