# ImageRepo


Hey Shopify team, I built an asp.net web application with a image repo built in to store and retrieve images from a mongoDB cluster. Tests are included in ImageRepoTests folder.
Clone the repo and follow the steps below to run:


![](showcase.gif)





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
* navigate to http://localhost:5000/Home/login

3) Run from visual studio using IIS Express, Docker, Project.

* If you have visual studio installed you should be able to open the project and run from there aswell
