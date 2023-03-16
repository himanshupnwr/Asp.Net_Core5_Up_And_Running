# Udemy-Asp.Net_Core5_Up_And_Running
Code Repository for Udemy Asp.net Core 5 Up and Running Course

username = admin@admin.com
passowrd = Admin@018

<img src="/Rocky/wwwroot/images/ViewBagVsViewdata.PNG" width="400"/>  <img src="/Rocky/wwwroot/images/ViewModel.PNG" width="400"/> 

# Tempdata

Tempdata can be used to store data between two consecutive requests.

Tempdata internally use session to store a data. So think of it as a short lived session.

Tempdata values must be type cast before use. Check for null values to avoid runtime error.

Tempdata can only be used to store one time information messages like error messages and validation messages.

command to add migration -> add-migration PushOrderHeaderAndDetailsToDB
command to update database -> update-database

Use .SUM of LINQ if trying to calculate sum instead of for and foreach loop