# Development - WIND MONITORING SYSTEM
-------------------------------------------------
GALE India test project
-------------------------------------------------

# Setup Notes

# DataBase Setup:

I. Web Application: 
-----------------------------------------------------------------------------------------------------------------------------------------

    1. I have uploaded both db schema script and the backup file inside the App Data folder. Please find those and make it your DB ready.
    2. In web config file please change your db user credentials . Where i have specified YOURDBUSER and YOURDBPWD
    3. Please enable nuget package manager for maintain all the packages stuffs. If any package is miss means please fix it with a help of nuger 
       Package manager.
    4. Once you done please build your solution .
    5. Make the Development.Web project is a startup project and registration page is the start up page.

II. Unit testing: (Development.Tests)
-----------------------------------------------------------------------------------------------------------------------------------------
    1. In App config file please change your db user credentials . Where i have specified YOURDBUSER and YOURDBPWD. Since i am using the same 
       DB for my Unit test project also. If we want to use seperate DB for the test cases we can change our db name in the configuration.


-Thank you for reading this and sorry for the inconvenience. :-). If any issues at the time of setup please let me know. I will solve the problems.


Thanks & Regards,

Viniston A. (SSE)

<a href="http://localhost:9091/viewType.html?buildTypeId=UnitTestDemo_CodeCheckin&guest=1">
<img src="http://localhost:9091/app/rest/builds/buildType:(id:UnitTestDemo_CodeCheckin)/statusIcon"/>
</a>





