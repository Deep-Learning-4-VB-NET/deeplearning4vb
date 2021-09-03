Imports System
Imports FileUtils = org.apache.commons.io.FileUtils
Imports IOFileFilter = org.apache.commons.io.filefilter.IOFileFilter
Imports RegexFileFilter = org.apache.commons.io.filefilter.RegexFileFilter
Imports TrueFileFilter = org.apache.commons.io.filefilter.TrueFileFilter
Imports FileUpdater = org.nd4j.fileupdater.FileUpdater
Imports CudaFileUpdater = org.nd4j.fileupdater.impl.CudaFileUpdater
Imports CommandLine = picocli.CommandLine

Namespace org.nd4j


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @CommandLine.Command(name = "version-update") public class VersionUpdater implements java.util.concurrent.Callable<Integer>
	Public Class VersionUpdater
		Implements Callable(Of Integer)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @CommandLine.@Option(names = {"--root-dir","-d"}) private java.io.File filePath;
		Private filePath As File
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @CommandLine.@Option(names = {"--cuda-version","-c"}) private String newCudaVersion;
		Private newCudaVersion As String
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @CommandLine.@Option(names = {"--cudnn-version","-cd"}) private String newCudnnVersion;
		Private newCudnnVersion As String
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @CommandLine.@Option(names = {"--javacpp-version","-jv"}) private String newJavacppVersion;
		Private newJavacppVersion As String
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @CommandLine.@Option(names = {"--update-type","-t"}) private String updateType = "cuda";
		Private updateType As String = "cuda"
		Private fileUpdater As FileUpdater


		Public Shared Sub Main(ParamArray ByVal args() As String)
			Dim commandLine As New CommandLine(New VersionUpdater())
			Environment.Exit(commandLine.execute(args))
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public System.Nullable<Integer> call() throws Exception
		Public Overrides Function [call]() As Integer?
			Try
				Select Case updateType
					Case "cuda"
						fileUpdater = New CudaFileUpdater(newCudaVersion, newJavacppVersion, newCudnnVersion)
						Console.WriteLine("Updating cuda version using cuda version " & newCudaVersion & " javacpp version " & newJavacppVersion & " cudnn version " & newCudnnVersion)
				End Select

				For Each f As File In FileUtils.listFilesAndDirs(filePath, New RegexFileFilter("pom.xml"), New IOFileFilterAnonymousInnerClass(Me))){if (fileUpdater.pathMatches(f)){fileUpdater.patternReplace(f
				Next f
			End Try
		End Function

				Private Class IOFileFilterAnonymousInnerClass
					Inherits IOFileFilter

					Private ReadOnly outerInstance As VersionUpdater

					Public Sub New(ByVal outerInstance As VersionUpdater)
						Me.outerInstance = outerInstance
					End Sub

					Public Overrides Function accept(ByVal file As File) As Boolean
						Return Not file.getName().Equals("target")
					End Function

					Public Overrides Function accept(ByVal file As File, ByVal s As String) As Boolean
						Return Not file.getName().Equals("target")
					End Function
				End Class
	End Class
			Catch e As Exception
				Console.WriteLine(e.ToString())
				Console.Write(e.StackTrace)
				Environment.Exit(1)
			End Try

			Return 0
		}
	}

End Namespace