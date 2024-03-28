Easy SQLite Backup, a simple Windows Gui tool for backing up SQLite databases.

![Feature Image](https://raw.githubusercontent.com/squawki/Easy-SQLite-Backup/master/screenshots/easySQLiteBackup_featureImage.png)


## Features
- Simple and intuitive interface
- Customizable backup options (Backup Compression, Write to Log, Notifications, Run On Start)
- Automated backup options ('Vacuum Into' Mode / File Copy)

## Getting Started
To use Easy SQLite Backup, follow these steps:
1. Download and extract the zip file
2. Launch the 'Easy SQLite Backup.exe' application
3. Select the SQLite database and destination by double-clicking on the white text boxes
4. Choose the required options, click save config (optional to keep config)
5. Click "Backup Now" to backup the database or set up the automated backup options

## Backup Modes
Easy SQLite Backup can backup your SQLite database using two different techniques
1. Online (Recommended): 'Vacuum Into' mode where the backup client will connect to the database and issue the 'vacuum into' command and destination which will generate a new optimised database file <a href="https://www.sqlite.org/lang_vacuum.html">[More Info]</a>
2. Offline: 'File Copy' essentially does a ctrl+c ctrl+v and copies the database file to the backup destination. The database file and size remain unchanged. This may cause file locking and brief interruption to writes.

## Backup Options
- File Compression: Backup File will be compressed within a zip file. This will generally substantially reduce the backup file size however will consume more cpu cycles
- Append Timestamp (highly recommended): Add '_dd/mm/yyyy_hhmmss' onto the file name to prevent risk over overwriting file 
- Start On Run: If the backup is enabled and config saved, the program will attempt to run the backup as per the countdown as soon as the program is started.
- Popup Notifications: When checked a notification will be generated from the task tray whenever the backup runs
- Write to log: Logfile is written to 'backup.log' for each backup event
## Contributing
We welcome contributions from the community. If you'd like to contribute to Easy SQLite Backup, please fork the repository and submit a pull request with your changes.

## Support
If you encounter any issues or have any questions, feel free to raise an issue.

## License
Easy SQLite Backup is licensed under the GPL-3.0 license. See the LICENSE file for details.
