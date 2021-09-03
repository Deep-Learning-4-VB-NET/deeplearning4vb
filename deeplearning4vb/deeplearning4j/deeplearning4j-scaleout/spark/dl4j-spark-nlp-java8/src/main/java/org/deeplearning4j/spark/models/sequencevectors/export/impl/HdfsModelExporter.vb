Imports NonNull = lombok.NonNull
Imports CompressionCodec = org.apache.hadoop.io.compress.CompressionCodec
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports SequenceElement = org.deeplearning4j.models.sequencevectors.sequence.SequenceElement
Imports org.deeplearning4j.spark.models.sequencevectors.export
Imports org.deeplearning4j.spark.models.sequencevectors.export

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

Namespace org.deeplearning4j.spark.models.sequencevectors.export.impl

	Public Class HdfsModelExporter(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)
		Implements SparkModelExporter(Of T)

		Protected Friend path As String
		Protected Friend codec As CompressionCodec

		Protected Friend Sub New()

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public HdfsModelExporter(@NonNull String path)
		Public Sub New(ByVal path As String)
			Me.New(path, Nothing)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public HdfsModelExporter(@NonNull String path, org.apache.hadoop.io.compress.CompressionCodec codec)
		Public Sub New(ByVal path As String, ByVal codec As CompressionCodec)
			Me.path = path
			Me.codec = codec
		End Sub

		Public Overridable Sub export(ByVal rdd As JavaRDD(Of ExportContainer(Of T))) Implements SparkModelExporter(Of T).export
			If codec Is Nothing Then
				rdd.saveAsTextFile(path)
			Else
				rdd.saveAsTextFile(path, codec.GetType())
			End If
		End Sub
	End Class

End Namespace