'
' Copyright (c) 2011 Makoto Ishida
' Please see the file MIT-LICENSE.txt for copying permission.
'

===========================================
S3 Backup Utility
===========================================

Amazon S3にファイルのバックアップを作成または削除する処理を自動化する為のツールです。
バックアップ先フォルダの下に自動的に日付毎のサブフォルダを作成します。
/delパラメータを指定すると、指定した日数より古いサブフォルダを自動的に削除します。Winsowsのタスクスケジューラー等に登録して定期実行される様に設定して下さい。

使い方:


  S3Back.exe /LB                            
    アカウント内のバケット一覧を表示します。

  S3Back.exe /LF bucketname foldername      
  　バケット内のオブジェクト（ファイルとフォルダ）の一覧を表示します。

  S3Back.exe /b  filename
	指定されたファイルをS3にコピーします。
	コピー先のバケット、フォルダー名、サブフォルダのプリフィックスは設定ファイルから取得します。

	サブフォルダのプリフィックスには現在日が自動的に付加されます。（日付の書式は設定ファイルのDateFormatで指定可能です。）

	例えば、バケット名が「MyBucket」、フォルダー名が「DailyBackup」、サブフォルダのプリフィックスが「TEST_」だった場合、ファイルは次の場所にコピーされます。

	  MyBucket>DailyBackup>TEST_20111220>filename
	
	1日に1回実行するとすると、サブフォルダが次のように増えていきます。

	  MyBucket>DailyBackup>TEST_20111220>filename
	  MyBucket>DailyBackup>TEST_20111221>filename
	  MyBucket>DailyBackup>TEST_20111222>filename
	  MyBucket>DailyBackup>TEST_20111223>filename

	ただし、MaxDaysで指定された日数よりも古いサブフォルダは、/delコマンドの実行時に自動的に削除されます。


  S3Back.exe /b  filename bucketname
	上と同じ。ただしバケット名を上書き指定。（設定ファイルよりも優先されます。）

  S3Back.exe /b  filename bucketname foldername
	上と同じ。ただしバケット名とフォルダ名を上書き指定。（設定ファイルよりも優先されます。）

  S3Back.exe /b  filename bucketname foldername subfolderprefix
	上と同じ。ただしバケット名、フォルダ名とサブフォルダのプリフィックスを上書き指定。（設定ファイルよりも優先されます。）



  S3Back.exe /del
	MaxDaysの日数よりも古いサブフォルダを削除します。

  S3Back.exe /del bucketname
	上と同じ。ただしバケット名を上書き指定。（設定ファイルよりも優先されます。）

  S3Back.exe /del bucketname foldername
	上と同じ。ただしバケット名とフォルダ名を上書き指定。（設定ファイルよりも優先されます。）

  S3Back.exe /del bucketname foldername subfolderprefix
	上と同じ。ただしバケット名、フォルダ名とサブフォルダのプリフィックスを上書き指定。（設定ファイルよりも優先されます。）

  S3Back.exe /del bucketname foldername subfolderprefix maxdays
	上と同じ。ただしバケット名、フォルダ名、サブフォルダのプリフィックス、およびMaxDaysを上書き指定。（設定ファイルよりも優先されます。）



  S3Back.exe /enc text_to_encrypt          
    設定ファイルに指定するアクセスキー、シークレットキーを暗号化するのに使用します。



===========================================
このツールを動かす前に「Ec2EbsSnap.exe.config」ファイルを編集してアクセスキーとシークレットキーをセットして下さい。

  EncryptedAWSAccessKey = あなたのAWSアカウントのアクセスキーを暗号化したもの
  EncryptedAWSSecretKey = あなたのAWSアカウントのシークレットキーを暗号化したもの

アクセスキーとシークレットキーの暗号化は下のコマンドで行います。
  Ec2EbsSnap.exe /enc "[あなたのアクセスキー]"
  Ec2EbsSnap.exe /enc "[あなたのシークレットキー]"

これらの結果表示される文字列を「Ec2EbsSnap.exe.config」ファイルにセットして下さい。
===========================================
