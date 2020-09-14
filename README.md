# HAML2ERP_Batch
small console app that uses the haml2erb service and runs it on every haml file in a project

It basically just calls https://haml2erb.org/ for every HAML file you have, saves the output as an ERB file and deletes the old HAML file.


# How to use

Compile the app by using dotnet run
Go into bin/debug/dotnetcore3.1
Run  .\HAML2ERP_Batch.exe <directory of the project you want to convert>



# logging
logs are appended to log.txt

