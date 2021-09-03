Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports ArrayUtils = org.apache.commons.lang3.ArrayUtils
Imports ColumnMetaData = org.datavec.api.transform.metadata.ColumnMetaData
Imports Schema = org.datavec.api.transform.schema.Schema
Imports NullWritable = org.datavec.api.writable.NullWritable
Imports Writable = org.datavec.api.writable.Writable

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

Namespace org.datavec.api.transform.join


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class Join implements java.io.Serializable
	<Serializable>
	Public Class Join

		''' <summary>
		''' Type of join<br>
		''' Inner: Return examples where the join column values occur in both
		''' LeftOuter: Return all examples from left data, whether there is a matching right value or not.
		''' (If not: right values will have NullWritable instead)<br>
		''' RightOuter: Return all examples from the right data, whether there is a matching left value or not.
		''' (If not: left values will have NullWritable instead)<br>
		''' FullOuter: return all examples from both left/right, whether there is a matching value from the other side or not.
		''' (If not: other values will have NullWritable instead)
		''' </summary>
		Public Enum JoinType
			Inner
			LeftOuter
			RightOuter
			FullOuter
		End Enum

		Private joinType As JoinType
		Private leftSchema As Schema
		Private rightSchema As Schema
		Private joinColumnsLeft() As String
		Private joinColumnsRight() As String


		Private Sub New(ByVal builder As Builder)
			Me.joinType = builder.joinType
			Me.leftSchema = builder.leftSchema
			Me.rightSchema = builder.rightSchema
			Me.joinColumnsLeft = builder.joinColumnsLeft_Conflict
			Me.joinColumnsRight = builder.joinColumnsRight_Conflict

			'Perform validation: ensure columns are correct, etc
			If joinType = Nothing Then
				Throw New System.ArgumentException("Join type cannot be null")
			End If
			If leftSchema Is Nothing Then
				Throw New System.ArgumentException("Left schema cannot be null")
			End If
			If rightSchema Is Nothing Then
				Throw New System.ArgumentException("Right schema cannot be null")
			End If
			If joinColumnsLeft Is Nothing OrElse joinColumnsLeft.Length = 0 Then
				Throw New System.ArgumentException("Invalid left join columns: " & (If(joinColumnsLeft Is Nothing, Nothing, java.util.Arrays.toString(joinColumnsLeft))))
			End If
			If joinColumnsRight Is Nothing OrElse joinColumnsRight.Length = 0 Then
				Throw New System.ArgumentException("Invalid right join columns: " & (If(joinColumnsRight Is Nothing, Nothing, java.util.Arrays.toString(joinColumnsRight))))
			End If

			'Check that the join columns actually appear in the schemas:
			For Each leftCol As String In joinColumnsLeft
				If Not leftSchema.hasColumn(leftCol) Then
					Throw New System.ArgumentException("Cannot perform join: left join column """ & leftCol & """ does not exist in left schema. All columns in left schema: " & leftSchema.getColumnNames())
				End If
			Next leftCol

			For Each rightCol As String In joinColumnsRight
				If Not rightSchema.hasColumn(rightCol) Then
					Throw New System.ArgumentException("Cannot perform join: right join column """ & rightCol & """ does not exist in right schema. All columns in right schema: " & rightSchema.getColumnNames())
				End If
			Next rightCol
		End Sub


		Public Class Builder

			Friend joinType As JoinType
			Friend leftSchema As Schema
			Friend rightSchema As Schema
'JAVA TO VB CONVERTER NOTE: The field joinColumnsLeft was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend joinColumnsLeft_Conflict() As String
'JAVA TO VB CONVERTER NOTE: The field joinColumnsRight was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend joinColumnsRight_Conflict() As String

			Public Sub New(ByVal type As JoinType)
				Me.joinType = type
			End Sub

			Public Overridable Function setSchemas(ByVal left As Schema, ByVal right As Schema) As Builder
				Me.leftSchema = left
				Me.rightSchema = right
				Return Me
			End Function


			''' @deprecated Use <seealso cref="setJoinColumns(String...)"/> 
			<Obsolete("Use <seealso cref=""setJoinColumns(String...)""/>")>
			Public Overridable Function setKeyColumns(ParamArray ByVal keyColumnNames() As String) As Builder
				Return setJoinColumns(keyColumnNames)
			End Function

			''' @deprecated Use <seealso cref="setJoinColumnsLeft(String...)"/> 
			<Obsolete("Use <seealso cref=""setJoinColumnsLeft(String...)""/>")>
			Public Overridable Function setKeyColumnsLeft(ParamArray ByVal keyColumnNames() As String) As Builder
				Return setJoinColumnsLeft(keyColumnNames)
			End Function

			''' @deprecated Use <seealso cref="setJoinColumnsRight(String...)"/> 
			<Obsolete("Use <seealso cref=""setJoinColumnsRight(String...)""/>")>
			Public Overridable Function setKeyColumnsRight(ParamArray ByVal keyColumnNames() As String) As Builder
				Return setJoinColumnsRight(keyColumnNames)
			End Function

			''' <summary>
			''' Specify the column(s) to join on.
			''' Here, we are assuming that both data sources have the same column names. If this is not the case,
			''' use <seealso cref="setJoinColumnsLeft(String...)"/> and <seealso cref="setJoinColumnsRight(String...)"/>.
			''' The idea: join examples where firstDataValues(joinColumNames[i]) == secondDataValues(joinColumnNames[i]) for all i </summary>
			''' <param name="joinColumnNames">    Name of the columns to use as the key to join on </param>
			Public Overridable Function setJoinColumns(ParamArray ByVal joinColumnNames() As String) As Builder
				JoinColumnsLeft = joinColumnNames
				Return setJoinColumnsRight(joinColumnNames)
			End Function

			''' <summary>
			''' Specify the names of the columns to join on, for the left data)
			''' The idea: join examples where firstDataValues(joinColumNamesLeft[i]) == secondDataValues(joinColumnNamesRight[i]) for all i </summary>
			''' <param name="joinColumnNames"> Names of the columns to join on (for left data) </param>
			Public Overridable Function setJoinColumnsLeft(ParamArray ByVal joinColumnNames() As String) As Builder
				Me.joinColumnsLeft_Conflict = joinColumnNames
				Return Me
			End Function

			''' <summary>
			''' Specify the names of the columns to join on, for the right data)
			''' The idea: join examples where firstDataValues(joinColumNamesLeft[i]) == secondDataValues(joinColumnNamesRight[i]) for all i </summary>
			''' <param name="joinColumnNames"> Names of the columns to join on (for left data) </param>
			Public Overridable Function setJoinColumnsRight(ParamArray ByVal joinColumnNames() As String) As Builder
				Me.joinColumnsRight_Conflict = joinColumnNames
				Return Me
			End Function

			Public Overridable Function build() As Join
				If leftSchema Is Nothing OrElse rightSchema Is Nothing Then
					Throw New System.InvalidOperationException("Cannot build Join: left and/or right schemas are null")
				End If
				Return New Join(Me)
			End Function
		End Class



		Public Overridable ReadOnly Property OutputSchema As Schema
			Get
				If leftSchema Is Nothing Then
					Throw New System.InvalidOperationException("Left schema is not set (null)")
				End If
				If rightSchema Is Nothing Then
					Throw New System.InvalidOperationException("Right schema is not set (null)")
				End If
				If joinColumnsLeft Is Nothing Then
					Throw New System.InvalidOperationException("Left key columns are not set (null)")
				End If
				If joinColumnsRight Is Nothing Then
					Throw New System.ArgumentException("Right key columns are not set (null")
				End If
    
				'Approach here: take the left schema, plus the right schema (excluding the key columns from the right schema)
				Dim metaDataOut As IList(Of ColumnMetaData) = New List(Of ColumnMetaData)(leftSchema.getColumnMetaData())
    
				Dim keySetRight As ISet(Of String) = New HashSet(Of String)()
				Collections.addAll(keySetRight, joinColumnsRight)
    
				For Each rightMeta As ColumnMetaData In rightSchema.getColumnMetaData()
					If keySetRight.Contains(rightMeta.Name) Then
						Continue For
					End If
					metaDataOut.Add(rightMeta)
				Next rightMeta
    
				Return leftSchema.newSchema(metaDataOut)
			End Get
		End Property

		''' <summary>
		''' Join the examples.
		''' Note: left or right examples may be null; if they are null, the appropriate NullWritables are inserted.
		''' </summary>
		''' <param name="leftExample"> </param>
		''' <param name="rightExample">
		''' @return </param>
		Public Overridable Function joinExamples(ByVal leftExample As IList(Of Writable), ByVal rightExample As IList(Of Writable)) As IList(Of Writable)

			Dim [out] As IList(Of Writable) = New List(Of Writable)()
			If leftExample Is Nothing Then
				If rightExample Is Nothing Then
					Throw New System.ArgumentException("Cannot join examples: Both examples are null (max 1 allowed to be null)")
				End If

				'Insert a set of null writables...
				'Complication here: the **key values** should still exist (we have to extract them from second value)
				Dim nLeft As Integer = leftSchema.numColumns()
				Dim leftNames As IList(Of String) = leftSchema.getColumnNames()
				Dim keysSoFar As Integer = 0
				For i As Integer = 0 To nLeft - 1
					Dim name As String = leftNames(i)
					If ArrayUtils.contains(joinColumnsLeft, name) Then
						'This would normally be where the left key came from...
						'So let's get the key value from the *right* example
						Dim rightKeyName As String = joinColumnsRight(keysSoFar)
						Dim idxOfRightKey As Integer = rightSchema.getIndexOfColumn(rightKeyName)
						[out].Add(rightExample(idxOfRightKey))
					Else
						'Not a key column, so just add a NullWritable
						[out].Add(NullWritable.INSTANCE)
					End If
				Next i
			Else
				CType([out], List(Of Writable)).AddRange(leftExample)
			End If

			Dim rightNames As IList(Of String) = rightSchema.getColumnNames()
			If rightExample Is Nothing Then
				'Insert a set of null writables...
				Dim nRight As Integer = rightSchema.numColumns()
				For i As Integer = 0 To nRight - 1
					Dim name As String = rightNames(i)
					If ArrayUtils.contains(joinColumnsRight, name) Then
						Continue For 'Skip the key column value
					End If
					[out].Add(NullWritable.INSTANCE)
				Next i
			Else
				'Add all values from right, except for key columns...
				For i As Integer = 0 To rightExample.Count - 1
					Dim name As String = rightNames(i)
					If ArrayUtils.contains(joinColumnsRight, name) Then
						Continue For 'Skip the key column value
					End If
					[out].Add(rightExample(i))
				Next i
			End If

			Return [out]
		End Function
	End Class

End Namespace