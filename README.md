This repository contains the source to a C# program designed to efficiently
download form, results and ratings data from the PuntingForm version 1
API.  You will need a valid ApiKey from PuntingForm to be able to use
this software.
The software was written using the Visual Studio IDE.  It requires
the NewtonsoftJSON and RestSharp libraries to operate.  These can be
downloaded with NuGet.
This program complies with the PuntingForm API download limits, specifically
there is at least 1 second between each API call, and the program only 
downloads data that has been updated.
