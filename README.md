# How to run it
1. clone the project
   git clone [https://github.com/rd003/YtBookStore.git](https://github.com/ParthiBanGitAcc/test9000)
2. open `appsettings.json` file and update connection string's `data source=your server name`
   
   "ConnectionStrings": {
  "DefaultConnection": "Server=LAPTOP-94RTSUV3\\SQLEXPRESS;Database=BookRentalDB;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
4. Delete `Migrations` folder
5. Open Tools > Package Manager > Package manager console
6. Run these 2 commands
    ```
     (i) add-migration init
     (ii) update-database
     ````
7. Now you can run this project
   
