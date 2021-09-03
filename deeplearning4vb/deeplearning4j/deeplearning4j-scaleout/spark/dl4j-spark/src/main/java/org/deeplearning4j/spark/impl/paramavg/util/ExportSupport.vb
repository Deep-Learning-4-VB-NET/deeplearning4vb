Imports System
Imports NonNull = lombok.NonNull
Imports FileSystem = org.apache.hadoop.fs.FileSystem
Imports JavaSparkContext = org.apache.spark.api.java.JavaSparkContext

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.deeplearning4j.spark.impl.paramavg.util


	Public Class ExportSupport
		''' <summary>
		''' Verify that exporting data is supported, and throw an informative exception if not.
		''' </summary>
		''' <param name="sc"> the Spark context </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static void assertExportSupported(@NonNull JavaSparkContext sc)
		Public Shared Sub assertExportSupported(ByVal sc As JavaSparkContext)
			If Not exportSupported(sc) Then
				Throw New Exception("Export training approach is not supported in the current environment. " & "This means that the default Hadoop file system is the local file system and Spark is running " & "in a non-local mode. You can fix this by either adding hadoop configuration to your environment " & "or using the Direct training approach. Configuring Hadoop can be done by adding config files (" & "https://spark.apache.org/docs/1.6.3/configuration.html#inheriting-hadoop-cluster-configuration" & ") or adding a setting to your SparkConf object with " & "`sparkConf.set(""spark.hadoop.fs.defaultFS"", ""hdfs://my-hdfs-host:9000"");`. Alternatively, " & "you can use some other non-local storage like S3.")
			End If
		End Sub

		''' <summary>
		''' Check if exporting data is supported in the current environment. Exporting is possible in two cases:
		''' - The master is set to local. In this case any file system, including local FS, will work for exporting.
		''' - The file system is not local. Local file systems do not work in cluster modes.
		''' </summary>
		''' <param name="sc"> the Spark context </param>
		''' <returns> if export is supported </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static boolean exportSupported(@NonNull JavaSparkContext sc)
		Public Shared Function exportSupported(ByVal sc As JavaSparkContext) As Boolean
			Try
				Return exportSupported(sc.master(), FileSystem.get(sc.hadoopConfiguration()))
			Catch ex As IOException
				Throw New Exception(ex)
			End Try
		End Function

		''' <summary>
		''' Check if exporting data is supported in the current environment. Exporting is possible in two cases:
		''' - The master is set to local. In this case any file system, including local FS, will work for exporting.
		''' - The file system is not local. Local file systems do not work in cluster modes.
		''' </summary>
		''' <param name="sparkMaster"> the Spark master </param>
		''' <param name="fs"> the Hadoop file system </param>
		''' <returns> if export is supported </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static boolean exportSupported(@NonNull String sparkMaster, @NonNull FileSystem fs)
		Public Shared Function exportSupported(ByVal sparkMaster As String, ByVal fs As FileSystem) As Boolean
			' Anything is supported with a local master. Regex matches 'local', 'local[DIGITS]' or 'local[*]'
			If sparkMaster.matches("^local(\[(\d+|\*)])?$") Then
				Return True
			End If
			' Clustered mode is supported as long as the file system is not a local one
			Return Not fs.getUri().getScheme().Equals("file")
		End Function
	End Class

End Namespace