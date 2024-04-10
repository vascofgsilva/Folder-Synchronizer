# Folder-Synchronizer
This program synchronizes two folders: source and replica. The program maintains a full, identical copy of source folder at replica folder.
## How it works
- Synchronization is one-way: only the content of the replica folder is modified to exactly match the content of the source folder;
- Synchronization is performed periodically.
- File and folder creation/copying/removal operations are logged to a file and to the console output;
- Folder paths, synchronization interval and log file path must be provided using command line arguments;
## How to use
Clone or download the files. In a CLI make sure you are at the root of the files and type the following command:

`dotnet run <source folder path> <replica folder path> <log file path> <interval in seconds>`

### Example:

`dotnet run "C:\Source" "C:\Replica" "C:\log.txt" 60`

This means the program will, every 60 seconds, synchronize the contents of the Replica folder to match the contents of the Source folder and log every operation in the log.txt file.
