Imports System.Collections.Generic
Imports FileUpdater = org.nd4j.fileupdater.FileUpdater

Namespace org.nd4j.fileupdater.impl


	Public Class CudaFileUpdater
		Implements FileUpdater

		Private cudaVersion As String
		Private javacppVersion As String
		Private cudnnVersion As String

		Public Sub New(ByVal cudaVersion As String, ByVal javacppVersion As String, ByVal cudnnVersion As String)
			Me.cudaVersion = cudaVersion
			Me.javacppVersion = javacppVersion
			Me.cudnnVersion = cudnnVersion
		End Sub

		Public Overridable Function patterns() As IDictionary(Of String, String) Implements FileUpdater.patterns
			Dim ret As IDictionary(Of String, String) = New Dictionary(Of String, String)()
			ret("\<artifactId\>nd4j-cuda-[0-9\.]+\<\/artifactId\>") = String.Format("<artifactId>nd4j-cuda-{0}</artifactId>",cudaVersion)
			ret("\<artifactId\>nd4j-cuda-[0-9\.]*-preset<\/artifactId\>") = String.Format("<artifactId>nd4j-cuda-{0}-preset</artifactId>",cudaVersion)
			ret("\<artifactId\>nd4j-cuda-[0-9\.]*-platform\<\/artifactId\>") = String.Format("<artifactId>nd4j-cuda-{0}-platform</artifactId>",cudaVersion)
			ret("\<artifactId\>deeplearning4j-cuda-[0-9\.]*\<\/artifactId\>") = String.Format("<artifactId>deeplearning4j-cuda-{0}</artifactId>",cudaVersion)
			ret("\<cuda.version\>[0-9\.]*<\/cuda.version\>") = String.Format("<cuda.version>{0}</cuda.version>",cudaVersion)
			ret("\<cudnn.version\>[0-9\.]*\<\/cudnn.version\>") = String.Format("<cudnn.version>{0}</cudnn.version>",cudnnVersion)
			ret("\<javacpp-presets.cuda.version\>[0-9\.]*<\/javacpp-presets.cuda.version\>") = String.Format("<javacpp-presets.cuda.version>{0}</javacpp-presets.cuda.version>",javacppVersion)
			Return ret
		End Function
	End Class

End Namespace