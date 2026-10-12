Imports System
Imports Configuration = org.apache.hadoop.conf.Configuration
Imports FSDataInputStream = org.apache.hadoop.fs.FSDataInputStream
Imports FileSystem = org.apache.hadoop.fs.FileSystem
Imports Path = org.apache.hadoop.fs.Path
Imports [Function] = org.apache.spark.api.java.function.Function
Imports Broadcast = org.apache.spark.broadcast.Broadcast
Imports DefaultHadoopConfig = org.datavec.spark.util.DefaultHadoopConfig
Imports SerializableHadoopConfig = org.datavec.spark.util.SerializableHadoopConfig
Imports ValidationResult = org.deeplearning4j.spark.util.data.ValidationResult
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
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

Namespace org.deeplearning4j.spark.util.data.validation


	Public Class ValidateDataSetFn
		Implements [Function](Of String, ValidationResult)

		Public Const BUFFER_SIZE As Integer = 4194304 '4 MB

		Private ReadOnly deleteInvalid As Boolean
		Private ReadOnly featuresShape() As Integer
		Private ReadOnly labelsShape() As Integer
		Private ReadOnly conf As Broadcast(Of SerializableHadoopConfig)
		<NonSerialized>
		Private fileSystem As FileSystem

		Public Sub New(ByVal deleteInvalid As Boolean, ByVal featuresShape() As Integer, ByVal labelsShape() As Integer)
			Me.New(deleteInvalid, featuresShape, labelsShape, Nothing)
		End Sub

		Public Sub New(ByVal deleteInvalid As Boolean, ByVal featuresShape() As Integer, ByVal labelsShape() As Integer, ByVal configuration As Broadcast(Of SerializableHadoopConfig))
			Me.deleteInvalid = deleteInvalid
			Me.featuresShape = featuresShape
			Me.labelsShape = labelsShape
			Me.conf = configuration
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.deeplearning4j.spark.util.data.ValidationResult call(String path) throws Exception
		Public Overrides Function [call](ByVal path As String) As ValidationResult
			If fileSystem Is Nothing Then
				Dim c As Configuration = If(conf Is Nothing, DefaultHadoopConfig.get(), conf.getValue().getConfiguration())
				Try
					fileSystem = FileSystem.get(New URI(path), c)
				Catch e As Exception
					Throw New Exception(e)
				End Try
			End If

			Dim ret As New ValidationResult()
			ret.setCountTotal(1)

			Dim shouldDelete As Boolean = False
			Dim loadSuccessful As Boolean = False
			Dim ds As New DataSet()
			Dim p As New Path(path)

			If fileSystem.isDirectory(p) Then
				ret.setCountTotal(0)
				Return ret
			End If

			If Not fileSystem.exists(p) Then
				ret.setCountMissingFile(1)
				Return ret
			End If

			Try
					Using inputStream As FSDataInputStream = fileSystem.open(p, BUFFER_SIZE)
					ds.load(inputStream)
					loadSuccessful = True
					End Using
			Catch t As Exception
				shouldDelete = deleteInvalid
				ret.setCountLoadingFailure(1)
			End Try

			Dim isValid As Boolean = loadSuccessful
			If loadSuccessful Then
				'Validate
				If ds.Features Is Nothing Then
					ret.setCountMissingFeatures(1)
					isValid = False
				Else
					If featuresShape IsNot Nothing AndAlso Not validateArrayShape(featuresShape, ds.Features) Then
						ret.setCountInvalidFeatures(1)
						isValid = False
					End If
				End If

				If ds.Labels Is Nothing Then
					ret.setCountMissingLabels(1)
					isValid = False
				Else
					If labelsShape IsNot Nothing AndAlso Not validateArrayShape(labelsShape, ds.Labels) Then
						ret.setCountInvalidLabels(1)
						isValid = False
					End If
				End If

				If Not isValid AndAlso deleteInvalid Then
					shouldDelete = True
				End If
			End If

			If isValid Then
				ret.setCountTotalValid(1)
			Else
				ret.setCountTotalInvalid(1)
			End If

			If shouldDelete Then
				fileSystem.delete(p, False)
				ret.setCountInvalidDeleted(1)
			End If

			Return ret
		End Function

		Protected Friend Shared Function validateArrayShape(ByVal featuresShape() As Integer, ByVal array As INDArray) As Boolean
			If featuresShape Is Nothing Then
				Return True
			End If

			If featuresShape.Length <> array.rank() Then
				Return False
			Else
				For i As Integer = 0 To featuresShape.Length - 1
					If featuresShape(i) <= 0 Then
						Continue For
					End If
					If featuresShape(i) <> array.size(i) Then
						Return False
					End If
				Next i
			End If
			Return True
		End Function
	End Class

End Namespace