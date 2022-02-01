# cn
Change Notifications. Windows command line app to show file system change notifications

Build using your favorite version of .net:

    c:\windows\microsoft.net\framework64\v4.0.30319\csc.exe /nowarn:0168 cn.cs

Usage:

    Usage: cn [root] [/d] [/f:[filter] [/s]
      Change Notifications
      arguments: [root]         Root of the watch. Default is current folder.
                 /d             Exclude directories; only notify on files.
                 /f:[filter]    Filename filter, e.g. *.txt
                 /s             Shallow, not deep watch of [root]
      examples: cn              (change notifications for current folder
                cn c:           (change notifications for volume C:
                cn c:\ /f:*.dll (change notifications for root of volume C:
                cn d:\pictures  (change notifications for the folder
      notes:    Even with /d, directory notifications will appear for deletes and ACL fails
