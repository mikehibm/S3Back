Imports System
Imports System.IO
Imports System.Collections.Generic
Imports System.Collections.Specialized
Imports System.Configuration

Imports Amazon
Imports Amazon.S3
Imports Amazon.S3.Model


Public Class S3Manager

    Private Shared Function GetAmazonS3() As AmazonS3
        Dim appConfig As NameValueCollection = ConfigurationManager.AppSettings

        Dim config As AmazonS3Config = New AmazonS3Config()

        Dim accessKey As String = Encrypt.Decrypt(appConfig("EncryptedAWSAccessKey"))
        Dim secretKey As String = Encrypt.Decrypt(appConfig("EncryptedAWSSecretKey"))
        Dim s3 As AmazonS3 = AWSClientFactory.CreateAmazonS3Client(accessKey, secretKey, config)

        Return s3
    End Function

    Private Shared Sub HandleError(ByVal ex As Exception)

        If ex.GetType.IsInstanceOfType(GetType(AmazonS3Exception)) Then
            Dim exAmazon As AmazonS3Exception = CType(ex, AmazonS3Exception)
            If exAmazon.ErrorCode.Equals("InvalidAccessKeyId") OrElse exAmazon.ErrorCode.Equals("InvalidSecurity") Then
                Console.WriteLine("Cannot sign in to S3.")
            Else
                Console.WriteLine("Caught Exception: " & exAmazon.Message)
                Console.WriteLine("Response Status Code: " & exAmazon.StatusCode.ToString())
                Console.WriteLine("Error Code: " & exAmazon.ErrorCode)
                Console.WriteLine("Request ID: " & exAmazon.RequestId)
                Console.WriteLine("XML: " & exAmazon.XML)
            End If
        Else
            Console.WriteLine("Error: " & ex.Message)
        End If

    End Sub


    ''' <summary>
    ''' Lists all existing buckets.
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub ListBuckets()
        Dim s3 As AmazonS3 = GetAmazonS3()

        Try
            Dim response As ListBucketsResponse = s3.ListBuckets()
            Dim list As List(Of Amazon.S3.Model.S3Bucket) = response.Buckets

            Console.WriteLine("You have " & list.Count.ToString() & " bucket(s).")
            Console.WriteLine()

            For Each itm As S3Bucket In list
                Console.WriteLine(itm.BucketName & " " & CDate(itm.CreationDate).ToString("yyyy/MM/dd HH:mm:ss"))
            Next

        Catch ex As Exception
            HandleError(ex)
        End Try
        Console.WriteLine()
    End Sub

    ''' <summary>
    ''' Lists all existing objects.
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub ShowObjects(bucketname As String, foldername As String)
        Try
            Dim list As List(Of S3Object) = GetObjects(bucketname, foldername)

            Console.WriteLine(list.Count.ToString() & " object(s) found.")
            Console.WriteLine()

            For Each itm As S3Object In list
                Console.WriteLine(itm.BucketName & "/" & itm.Key _
                                    & " " & CDate(itm.LastModified).ToString("yyyy/MM/dd HH:mm:ss") _
                                    & " " & itm.Size.ToString("#,0"))
            Next

        Catch ex As Exception
            HandleError(ex)
        End Try
        Console.WriteLine()
    End Sub

    ''' <summary>
    ''' Lists all existing buckets.
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Function GetObjects(bucketname As String, foldername As String) As List(Of S3Object)
        Dim list As List(Of S3Object) = Nothing
        Dim s3 As AmazonS3 = GetAmazonS3()

        Dim request As New ListObjectsRequest()
        request.BucketName = bucketname
        request.Prefix = foldername

        Dim response As ListObjectsResponse = s3.ListObjects(request)
        list = response.S3Objects

        Return list
    End Function


    Public Shared Function CreateFolder(bucketname As String, foldername As String) As Boolean
        Dim result As Boolean = False
        Dim s3 As AmazonS3 = GetAmazonS3()

        Try
            Console.Write("Creating folder '" & foldername & "' ... ")

            Dim request As New PutObjectRequest()
            request.BucketName = bucketname
            request.Key = foldername
            request.ContentBody = ""

            Dim response As PutObjectResponse = s3.PutObject(request)
            Console.WriteLine("Success.")
            result = True

        Catch ex As Exception
            HandleError(ex)
        End Try
        Console.WriteLine()

        Return result
    End Function

    Public Shared Function DeleteObject(bucketname As String, key As String) As Boolean
        Dim result As Boolean = False
        Dim s3 As AmazonS3 = GetAmazonS3()

        Try
            Console.Write("Deleting object '" & key & "' ... ")

            Dim request As New DeleteObjectRequest()
            request.BucketName = bucketname
            request.Key = key

            Dim response As DeleteObjectResponse = s3.DeleteObject(request)
            If String.IsNullOrEmpty(response.ResponseXml) Then
                Console.WriteLine("Success.")
                result = True
            Else
                Throw New Exception(response.ToString())
            End If

        Catch ex As Exception
            HandleError(ex)
        End Try
        Console.WriteLine()

        Return result
    End Function

    Public Shared Function UploadFile(bucketname As String, foldername As String, filepath As String) As Boolean
        Dim result As Boolean = False
        Dim s3 As AmazonS3 = GetAmazonS3()

        Try
            Console.Write("Uploading file '" & filepath & "' ... ")

            Dim filename As String = System.IO.Path.GetFileName(filepath)

            Dim request As New PutObjectRequest()
            request.BucketName = bucketname
            request.Key = foldername & "/" & filename
            request.FilePath = filepath

            Dim timeout_minutes As Integer = CInt(Val(ConfigurationManager.AppSettings("UploadTimeOutMinutes")))
            If timeout_minutes <= 0 Then timeout_minutes = 60
            Dim span As New TimeSpan(0, timeout_minutes, 0)
            request.Timeout = CInt(span.TotalMilliseconds)                   'Upload timeout in milliseconds.

            Dim response As PutObjectResponse = s3.PutObject(request)
            Console.WriteLine("Success.")
            result = True

        Catch ex As Exception
            HandleError(ex)
        End Try
        Console.WriteLine()

        Return result
    End Function

    Public Shared Function DeleteFile(bucketname As String, foldername As String, filepath As String) As Boolean
        Dim result As Boolean = False
        Dim s3 As AmazonS3 = GetAmazonS3()

        Try
            Console.Write("Deleting file '" & filepath & "' ... ")

            Dim filename As String = System.IO.Path.GetFileName(filepath)

            Dim request As New DeleteObjectRequest()
            request.BucketName = bucketname
            request.Key = foldername & "/" & filename

            Dim response As DeleteObjectResponse = s3.DeleteObject(request)
            Console.WriteLine("Success.")
            result = True

        Catch ex As Exception
            HandleError(ex)
        End Try
        Console.WriteLine()

        Return result
    End Function


End Class
