using System;
using System.Collections;
using System.IO;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.ComponentModel;

class ChangeNotifications
{
    static bool excludeDirectories = false;

    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    private static void Watch( string root, bool deepWatch, string filter )
    {
        using ( FileSystemWatcher watcher = new FileSystemWatcher() )
        {
            watcher.InternalBufferSize = 65536;

            watcher.Path = System.IO.Path.GetFullPath( root );
            Console.WriteLine( "Watching " + watcher.Path );
    
            watcher.NotifyFilter = // NotifyFilters.LastAccess |
                                   NotifyFilters.CreationTime |
                                   NotifyFilters.Size |
                                   NotifyFilters.LastWrite |
                                   NotifyFilters.FileName |
                                   NotifyFilters.Security |
                                   NotifyFilters.Attributes |
                                   NotifyFilters.DirectoryName;

            watcher.Filter = filter;
            watcher.IncludeSubdirectories = deepWatch;
    
            watcher.Changed += OnChanged;
            watcher.Created += OnChanged;
            watcher.Deleted += OnChanged;
            watcher.Renamed += OnRenamed;
            watcher.Error += OnError;
    
            watcher.EnableRaisingEvents = true;
    
            do
            {
                Thread.Sleep( 1000 );
            } while ( true );
        }
    } //Watch

    private static bool IsDirectory( string s )
    {
        bool isDirectory = false;

        try
        {
            isDirectory = ( 0 != ( FileAttributes.Directory & File.GetAttributes( s ) ) );
        }
        catch (Exception ex)
        {
        }

        return isDirectory;
    } //IsDirectory
    
    private static void OnChanged( object source, FileSystemEventArgs e )
    {
        if ( excludeDirectories && IsDirectory( e.FullPath ) )
            return;

        Console.WriteLine( " " + e.ChangeType + " " + e.FullPath );
    } //OnChanged
    
    private static void OnRenamed( object source, RenamedEventArgs e )
    {
        if ( excludeDirectories && IsDirectory( e.FullPath ) )
            return;

        Console.WriteLine( " Renamed " + e.OldFullPath + " to " + e.FullPath );
    } //OnRenamed

    private static void OnError( object source, ErrorEventArgs e )
    {
        if ( e.GetException().GetType() == typeof(InternalBufferOverflowException) )
        {
            Console.WriteLine( "(too many changes; some lost)" );
        }
    } //OnError

    static void Usage()
    {
        Console.WriteLine( @"Usage: cn [root] [/d] [/f:[filter] [/s]" );
        Console.WriteLine( @"  Change Notifications" );
        Console.WriteLine( @"  arguments: [root]         Root of the watch. Default is current folder." );
        Console.WriteLine( @"             /d             Exclude directories; only notify on files." );
        Console.WriteLine( @"             /f:[filter]    Filename filter, e.g. *.txt" );
        Console.WriteLine( @"             /s             Shallow, not deep watch of [root]" );
        Console.WriteLine( @"  examples: cn              (change notifications for current folder" );
        Console.WriteLine( @"            cn c:           (change notifications for volume C:" );
        Console.WriteLine( @"            cn c:\ /f:*.dll (change notifications for root of volume C:" );
        Console.WriteLine( @"            cn d:\pictures  (change notifications for the folder" );
        Console.WriteLine( @"  notes:    Even with /d, directory notifications will appear for deletes and ACL fails" );

        Environment.Exit(1);
    } //Usage

    static void Main( string[] args )
    {
        string notificationRoot = null;
        bool deepWatch = true;
        string filter = "";

        for ( int i = 0; i < args.Length; i++ )
        {
            string arg = args[i];

            if ( '-' == arg[0] || '/' == arg[0] )
            {
                string argUpper = arg.ToUpper();
                char c = argUpper[1];

                if ( 'D' == c )
                    excludeDirectories = true;
                else if ( 'F' == c )
                {
                    if ( ':' != argUpper[2] )
                        Usage();

                    filter = arg.Substring( 3 );
                }
                else if ( 'S' == c )
                    deepWatch = false;
                else
                    Usage();
            }
            else
            {
                if ( notificationRoot != null )
                    Usage();

                notificationRoot = arg;
            }
        }

        if ( null == notificationRoot )
            notificationRoot = @".\";

        try
        {
            Watch( notificationRoot, deepWatch, filter );
        }
        catch (Exception e)
        {
            Console.WriteLine( "cn caught an exception {0}", e.ToString() );
            Usage();
        }
    } //Main
} //ChangeNotifications

