# TestProject

Steps to run this project:
1).Download the project from github
2).Open Project in Visual Studio 2019 
3).Go to TestProject->DataAccess->Email class
   change loginemail,fromemail and loginpassword(use your gmail account id and password)

   Note:- whatever email passes here, go to its security setting and enable less secure app.
4).Create a database in SQL Server Management Studio
5).Change Configuration setting.Go To TestProject->appsettings.json
   change datasource name according to your servername and also change initial catalog/databasename(The name with which you created your database)
6).Open Pacakge Manager Console(Tools->NuGet Package Manager->Package Manager Console)
7)."add-migration init"  run this command
8).then "update-database" run this command
7).then project is ready to run

   
