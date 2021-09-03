Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports NonNull = lombok.NonNull
Imports Distance = org.datavec.api.transform.Distance
Imports Transform = org.datavec.api.transform.Transform
Imports ColumnMetaData = org.datavec.api.transform.metadata.ColumnMetaData
Imports DoubleMetaData = org.datavec.api.transform.metadata.DoubleMetaData
Imports Schema = org.datavec.api.transform.schema.Schema
Imports DoubleWritable = org.datavec.api.writable.DoubleWritable
Imports NDArrayWritable = org.datavec.api.writable.NDArrayWritable
Imports Writable = org.datavec.api.writable.Writable
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
Imports JsonProperty = org.nd4j.shade.jackson.annotation.JsonProperty

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

Namespace org.datavec.api.transform.ndarray


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class NDArrayDistanceTransform implements org.datavec.api.transform.Transform
	<Serializable>
	Public Class NDArrayDistanceTransform
		Implements Transform

		Private newColumnName As String
		Private distance As Distance
		Private firstCol As String
		Private secondCol As String

'JAVA TO VB CONVERTER NOTE: The field inputSchema was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private inputSchema_Conflict As Schema

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public NDArrayDistanceTransform(@JsonProperty("newColumnName") @NonNull String newColumnName, @JsonProperty("distance") @NonNull Distance distance, @JsonProperty("firstCol") @NonNull String firstCol, @JsonProperty("secondCol") @NonNull String secondCol)
		Public Sub New(ByVal newColumnName As String, ByVal distance As Distance, ByVal firstCol As String, ByVal secondCol As String)
			Me.newColumnName = newColumnName
			Me.distance = distance
			Me.firstCol = firstCol
			Me.secondCol = secondCol
		End Sub


		Public Overrides Function ToString() As String
			Return "NDArrayDistanceTransform(newColumnName=""" & newColumnName & """,distance=" & distance & ",firstCol=" & firstCol & ",secondCol=" & secondCol & ")"
		End Function

		Public Overridable WriteOnly Property InputSchema As Schema
			Set(ByVal inputSchema As Schema)
				If Not inputSchema.hasColumn(firstCol) Then
					Throw New System.InvalidOperationException("Input schema does not have first column: " & firstCol)
				End If
				If Not inputSchema.hasColumn(secondCol) Then
					Throw New System.InvalidOperationException("Input schema does not have first column: " & secondCol)
				End If
    
				Me.inputSchema_Conflict = inputSchema
			End Set
		End Property

		Public Overridable Function map(ByVal writables As IList(Of Writable)) As IList(Of Writable) Implements Transform.map
			Dim idxFirst As Integer = inputSchema_Conflict.getIndexOfColumn(firstCol)
			Dim idxSecond As Integer = inputSchema_Conflict.getIndexOfColumn(secondCol)

			Dim arr1 As INDArray = DirectCast(writables(idxFirst), NDArrayWritable).get()
			Dim arr2 As INDArray = DirectCast(writables(idxSecond), NDArrayWritable).get()

			Dim d As Double
			Select Case distance
				Case Distance.COSINE
					d = Transforms.cosineSim(arr1, arr2)
				Case Distance.EUCLIDEAN
					d = Transforms.euclideanDistance(arr1, arr2)
				Case Distance.MANHATTAN
					d = Transforms.manhattanDistance(arr1, arr2)
				Case Else
					Throw New System.NotSupportedException("Unknown or not supported distance metric: " & distance)
			End Select

			Dim [out] As IList(Of Writable) = New List(Of Writable)(writables.Count + 1)
			CType([out], List(Of Writable)).AddRange(writables)
			[out].Add(New DoubleWritable(d))

			Return [out]
		End Function

		Public Overridable Function mapSequence(ByVal sequence As IList(Of IList(Of Writable))) As IList(Of IList(Of Writable)) Implements Transform.mapSequence
			Dim [out] As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			For Each l As IList(Of Writable) In sequence
				[out].Add(map(l))
			Next l
			Return [out]
		End Function

		Public Overridable Function map(ByVal input As Object) As Object Implements Transform.map
			Throw New System.NotSupportedException("Not yet implemented")
		End Function

		Public Overridable Function mapSequence(ByVal sequence As Object) As Object Implements Transform.mapSequence
			Throw New System.NotSupportedException("Not yet implemented")
		End Function

		Public Overrides Function transform(ByVal inputSchema As Schema) As Schema
			Dim newMeta As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)(inputSchema.getColumnMetaData())
			newMeta.Add(New DoubleMetaData(newColumnName))
			Return inputSchema.newSchema(newMeta)
		End Function

		Public Overridable Function outputColumnName() As String
			Return newColumnName
		End Function

		Public Overridable Function outputColumnNames() As String()
			Return New String() {outputColumnName()}
		End Function

		Public Overridable Function columnNames() As String()
			Return New String() {firstCol, secondCol}
		End Function

		Public Overridable Function columnName() As String
			Return columnNames()(0)
		End Function
	End Class

End Namespace