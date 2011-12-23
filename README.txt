'
' Copyright (c) 2011 Makoto Ishida
' Please see the file MIT-LICENSE.txt for copying permission.
'


===========================================
S3 Backup Utility
===========================================

This tool is used for creating and deleting backups on your Amazon S3 account.

Usage:

  S3Back.exe /LB                            
    List all buckets.

  S3Back.exe /LF bucketname foldername      
    List all objects that starts with a foldername in a bucket.

  S3Back.exe /b  filename
    Copy a file to S3. Bucket name, folder name and subfolder prefix are taken from .config file.

  S3Back.exe /b  filename bucketname
    Copy a file to S3. Folder name and subfolder prefix are taken from .config file.

  S3Back.exe /b  filename bucketname foldername
    Copy a file to S3. Subfolder prefix are taken from .config file.

  S3Back.exe /b  filename bucketname foldername subfolderprefix
    Copy a file to S3. 


  S3Back.exe /del
    Delete subfolders that are older than MaxDays specified in the config file.

  S3Back.exe /del bucketname
    Delete subfolders that are older than MaxDays specified in the config file.
	Uses bucket name specified in the parameter rather than the config file. 

  S3Back.exe /del bucketname foldername
    Delete subfolders that are older than MaxDays specified in the config file.
	Uses bucket name and folder name specified in the parameter rather than the config file. 

  S3Back.exe /del bucketname foldername subfolderprefix
    Delete subfolders that are older than MaxDays specified in the config file.
	Uses bucket name, folder name and subfolder prefix specified in the parameter rather than the config file. 

  S3Back.exe /del bucketname foldername subfolderprefix maxdays
    Delete subfolders that are older than MaxDays specified in the config file.
	Uses bucket name, folder name, subfolder prefix and max days specified in the parameter rather than the config file. 


  S3Back.exe /enc text_to_encrypt          
    Encrypt a text to be used in config file.


===========================================
YOU SHOULD EDIT S3Back.exe.config file first in order to access your Amazon S3 account.

  EncryptedAWSAccessKey = Your ENCRYPTED AWS Access Key
  EncryptedAWSSecretKey = Your ENCRYPTED AWS Secret Key

You can encrypt your keys by executing: 
  S3Back.exe /enc "[Your Access Key]"
  S3Back.exe /enc "[Your Secret Key]"

And then put the results in the S3Back.exe.config file.
===========================================




