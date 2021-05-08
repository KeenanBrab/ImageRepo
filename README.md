# ImageRepo

1) Running From command line with Docker Windows:

* From the ImageRepo-master top folder run:
* docker build . -t repo
* docker run -p 8080:80 repo:latest
* then navigate to http://localhost:8080/Home/login

2) Running from command line with dotnet:

* Install .net 5
* navigate to ImageRepo-master/ImageRepo/
* dotnet build
* dotnet run
* navigate to http://localhost:5000

3) Run from visual studio using IIS Express, Docker, Project.

* If you have visual studio installed you should be able to open the project and run from there aswell
