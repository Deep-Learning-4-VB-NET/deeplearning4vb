Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports Configuration = org.apache.hadoop.conf.Configuration
Imports FSDataOutputStream = org.apache.hadoop.fs.FSDataOutputStream
Imports FileSystem = org.apache.hadoop.fs.FileSystem
Imports Path = org.apache.hadoop.fs.Path
Imports VoidFunction = org.apache.spark.api.java.function.VoidFunction
Imports Broadcast = org.apache.spark.broadcast.Broadcast
Imports DefaultHadoopConfig = org.datavec.spark.util.DefaultHadoopConfig
Imports SerializableHadoopConfig = org.datavec.spark.util.SerializableHadoopConfig
Imports UIDProvider = org.deeplearning4j.core.util.UIDProvider
Imports DataSet = org.nd4j.linalg.dataset.DataSet

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

Namespace org.deeplearning4j.spark.data


	Public Class DataSetExportFunction
		Implements VoidFunction(Of IEnumerator(Of DataSet))

		Private ReadOnly outputDir As URI
		Private ReadOnly conf As Broadcast(Of SerializableHadoopConfig)
		Private uid As String = Nothing

		Private outputCount As Integer

		Public Sub New(ByVal outputDir As URI)
			Me.New(outputDir, Nothing)
		End Sub

		Public Sub New(ByVal outputDir As URI, ByVal configuration As Broadcast(Of SerializableHadoopConfig))
			Me.outputDir = outputDir
			Me.conf = configuration
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void call(java.util.Iterator<org.nd4j.linalg.dataset.DataSet> iter) throws Exception
		Public Overrides Sub [call](ByVal iter As IEnumerator(Of DataSet))
			Dim jvmuid As String = UIDProvider.JVMUID
			uid = Thread.CurrentThread.getId() & jvmuid.Substring(0, Math.Min(8, jvmuid.Length))

			Dim c As Configuration = If(conf Is Nothing, DefaultHadoopConfig.get(), conf.getValue().getConfiguration())

			Do While iter.MoveNext()
				Dim [next] As DataSet = iter.Current

'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: String filename = "dataset_" + uid + "_" + (outputCount++) + ".bin";
				Dim filename As String = "dataset_" & uid & "_" & (outputCount) & ".bin"
					outputCount += 1

				Dim path As String = outputDir.getPath()
				Dim uri As New URI(path & (If(path.EndsWith("/", StringComparison.Ordinal) OrElse path.EndsWith("\", StringComparison.Ordinal), "", "/")) & filename)
				Dim file As FileSystem = FileSystem.get(uri, c)
				Using [out] As org.apache.hadoop.fs.FSDataOutputStream = file.create(New org.apache.hadoop.fs.Path(uri))
					[next].save([out])
				End Using
			Loop
		End Sub
	End Class

End Namespace