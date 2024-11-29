# Inventory-Management-System

## To run this project
Open Inventory in Visual Studio 2022 (open **Inventory.sln** file in Inventory directory), and open InventoryAPI in Visual Studio Code (go to this directory then right click in directory and select **Open With Code**), and MySQL Workbench

### Dependencies for Visual Studio 2022
You have to install the neccessary packages for the Visual Studio, you can see them when you go to **Solution Explorer**, select **dependencies**, then right click on **packages**, select **Manage NuGet Packages** option, a tab will open in Visual Studio, from there select Updates, and update all packages

**Note:** If updates does not work, then select a package manually and update it to version below the latest, not the latest one

### Dependencies for VS Code
**1) Install Angular CLI**
Use this command on terminal:
npm install -g @angular/cli

**2) Clone the Repository**
Use this command on terminal: git clone https://github.com/Lala-Mumtaz-Ali/Inventory-Management-System/tree/main
then this: cd your-angular-project

**3) Install Project Dependencies**
Use this command on terminal: npm install

**4) Run the project**
Use this command on terminal: ng serve -o


### For MySQL Workbench
I have uploaded the dumps folder, just import them in MySQL Workbench.
#### Follow these steps for successfull dumping
Download the dump folder if you have not from the repository.
For your ease, you can create a new database named **mumtaz** , and then go to visual studio 2022 where you are currently running the Inventory.sln, then from the Solution Explorer, delete all migrations from the Migration folder, from there, on the TOOL BAR(upper most tool-options bar), select **Tools**, then hover over **NuGet Package Manager**, then select **Package Manager Console**, then do this on the console:
Add-Migration InitialMigration
Update-Database
You will get a schema in your database mumtaz, there you will see the tables. From workbench, right click on the table named LotMovements, and select **Alter** from there you have to remove the column named Product_id from this table. (You can get further help from CHATGPT if you get stuck here).
Then Select **Server** in workbench, from there select **Data Import**, tick the option **Import from Self-Contained File**, navigate to the dump folder on your pc then select the files the one by one, after selecting a file. In **Default Schema to be Imported To**, set **Default Target Schema** to **mumtaz** (database in our case), then click on **Import**.
Like this you can import the data to the schema.


### To run the project successfully
You have run both Visual Studio Code and Visual Studio 2022 programs simultaneosly, you can run the Visual Studio Code by this command on its terminal **ng serve -o**, and you also have to run the VS 2022 program, just simple do this. **Hold CTRL then press F5 (CTRL+F5)**, a console will pop run, keep the console in the background.
