'
' Copyright (c) 2011 Makoto Ishida
' Please see the file MIT-LICENSE.txt for copying permission.
'

Imports System.Configuration
Imports System.Configuration.ConfigurationManager

Imports Amazon
Imports Amazon.S3
Imports Amazon.S3.Model



Module Module1



    ''' <summary>
    ''' </summary>
    ''' <param name="args"></param>
    ''' <remarks>
    ''' 
    ''' Example: 
    '''   S3Back.exe  /b "C:\PATH\DATA.ZIP"
    '''   S3Back.exe  /DEL "AnyFolderName" "DailyBackup" "" 5
    ''' 
    ''' </remarks>
    Sub Main(ByVal args() As String)
        Console.WriteLine("===========================================")
        Console.WriteLine("S3 Backup Utility")
        Console.WriteLine("===========================================")

        If args.Count = 1 Then
            Select Case args(0).ToUpper
                Case "/LB" : ListBuckets()
                Case "/LF" : ListObjects("", "")
                Case "/DEL" : DeleteFolder("", "", "", "")
                Case Else : ShowHelp()
            End Select

        ElseIf args.Count = 2 Then
            Select Case args(0).ToUpper
                Case "/B" : UploadFile("", "", "", args(1))
                Case "/ENC" : ShowEncrypted(args(1))
                Case "/DEC" : ShowDecrypted(args(1))
                Case "/DEL" : DeleteFolder(args(1), "", "", "")
                Case Else : ShowHelp()
            End Select

        ElseIf args.Count = 3 Then
            Select Case args(0).ToUpper
                Case "/B" : UploadFile(args(2), "", "", args(1))
                Case "/LF" : ListObjects(args(1), args(2))
                Case "/DEL" : DeleteFolder(args(1), args(2), "", "")
                Case Else : ShowHelp()
            End Select

        ElseIf args.Count = 4 Then
            Select Case args(0).ToUpper
                Case "/B" : UploadFile(args(2), args(3), "", args(1))
                Case "/DEL" : DeleteFolder(args(1), args(2), args(3), "")
                Case Else : ShowHelp()
            End Select

        ElseIf args.Count = 5 Then
            Select Case args(0).ToUpper
                Case "/B" : UploadFile(args(2), args(3), args(4), args(1))
                Case "/DEL" : DeleteFolder(args(1), args(2), args(3), args(4))
                Case Else : ShowHelp()
            End Select

        Else
            ShowHelp()
        End If

#If DEBUG Then
        Console.WriteLine("Press any key.")
        Console.ReadKey()
#End If

    End Sub

    Private Sub ShowHelp()
        Console.WriteLine("Usage:")
        Console.WriteLine("  /LB                            : List all buckets.")
        Console.WriteLine("  /LF bucketname foldername      : List all objects.")
        Console.WriteLine()
        Console.WriteLine("  /b  filename ")
        Console.WriteLine("  /b  filename bucketname ")
        Console.WriteLine("  /b  filename bucketname foldername ")
        Console.WriteLine("  /b  filename bucketname foldername subfolderprefix ")
        Console.WriteLine()
        Console.WriteLine("  /del ")
        Console.WriteLine("  /del bucketname ")
        Console.WriteLine("  /del bucketname foldername ")
        Console.WriteLine("  /del bucketname foldername subfolderprefix ")
        Console.WriteLine("  /del bucketname foldername subfolderprefix maxdays ")
        Console.WriteLine()
        Console.WriteLine("  /enc text_to_encrypt          : Encrypt a text to be used in config file.")

    End Sub

    Private Sub ShowEncrypted(ByVal str As String)
        Console.WriteLine("Encrypted: ")
        Console.WriteLine(Encrypt.Encrypt(str))
    End Sub

    Private Sub ShowDecrypted(ByVal str As String)
        Console.WriteLine("Decrypted: ")
        Console.WriteLine(Encrypt.Decrypt(str))
    End Sub

    Private Sub ListBuckets()
        Console.WriteLine("Existing buckets:")
        S3Manager.ListBuckets()
    End Sub

    Private Sub ListObjects(bucketname As String, foldername As String)
        If String.IsNullOrEmpty(bucketname) Then bucketname = AppSettings("BucketName")
        If String.IsNullOrEmpty(foldername) Then foldername = AppSettings("FolderName")

        Console.WriteLine("Existing objects:")
        S3Manager.ShowObjects(bucketname, foldername)
    End Sub

    Private Function UploadFile( _
                        bucketname As String, _
                        foldername As String, _
                        subfolderprefix As String, _
                        filepath As String) As Boolean

        If String.IsNullOrEmpty(bucketname) Then bucketname = AppSettings("BucketName")
        If String.IsNullOrEmpty(foldername) Then foldername = AppSettings("FolderName")
        If String.IsNullOrEmpty(subfolderprefix) Then subfolderprefix = AppSettings("SubFolderPrefix")

        Dim dateformat As String = AppSettings("DateFormat")
        If String.IsNullOrEmpty(dateformat) Then dateformat = "yyyyMMdd"
        Dim sdate As String = Date.Today.ToString(dateformat)
        Dim newfoldername = foldername & "/" & subfolderprefix & sdate

        If Not S3Manager.UploadFile(bucketname, newfoldername, filepath) Then
            Return False
        End If

        Return True
    End Function

    Private Sub DeleteFolder( _
                        bucketname As String, _
                        foldername As String, _
                        subfolderprefix As String, _
                        maxdays As String)

        If String.IsNullOrEmpty(bucketname) Then bucketname = AppSettings("BucketName")
        If String.IsNullOrEmpty(foldername) Then foldername = AppSettings("FolderName")
        If String.IsNullOrEmpty(subfolderprefix) Then subfolderprefix = AppSettings("SubFolderPrefix")
        If String.IsNullOrEmpty(maxdays) Then maxdays = AppSettings("MaxDays")

        Dim imaxdays As Integer = CInt(Val(maxdays))
        If imaxdays <= 0 Then Exit Sub

        Console.WriteLine("Deleting folders older than " & imaxdays.ToString & " days...")

        Dim dateformat As String = AppSettings("DateFormat")
        If String.IsNullOrEmpty(dateformat) Then dateformat = "yyyyMMdd"

        Dim srchprefix As String = foldername & "/" & subfolderprefix

        Dim slimitdate As String = Date.Today.AddDays((imaxdays + 1) * -1).ToString(dateformat)
        Dim object_date As String = "", p As Integer

        Dim objectList As List(Of S3Object) = S3Manager.GetObjects(bucketname, srchprefix)
        For Each itm As S3Object In objectList
            If itm.Key.Length > srchprefix.Length Then
                object_date = itm.Key.Substring(srchprefix.Length)
                If object_date.EndsWith("/") Then object_date = object_date.Substring(0, object_date.Length - 1)
                p = object_date.IndexOf("/")
                If p >= 0 Then
                    object_date = object_date.Substring(0, p)
                End If
                If object_date.CompareTo(slimitdate) <= 0 Then
                    S3Manager.DeleteObject(bucketname, itm.Key)
                End If
            End If
        Next

    End Sub

End Module
