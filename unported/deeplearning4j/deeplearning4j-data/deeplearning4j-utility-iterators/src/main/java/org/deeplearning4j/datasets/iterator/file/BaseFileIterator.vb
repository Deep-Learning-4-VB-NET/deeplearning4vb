Imports System
Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Setter = lombok.Setter
Imports FileUtils = org.apache.commons.io.FileUtils
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports CompactHeapStringList = org.nd4j.common.collection.CompactHeapStringList
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports MathUtils = org.nd4j.common.util.MathUtils

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

Namespace org.deeplearning4j.datasets.iterator.file


	Public MustInherit Class BaseFileIterator(Of T, P)
		Implements IEnumerator(Of T)

		Protected Friend ReadOnly list As IList(Of String)
		Protected Friend ReadOnly batchSize As Integer
		Protected Friend ReadOnly rng As Random

		Protected Friend order() As Integer
		Protected Friend position As Integer

		Private partialStored As T
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter protected P preProcessor;
		Protected Friend preProcessor As P


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected BaseFileIterator(@NonNull File rootDir, int batchSize, String... validExtensions)
		Protected Friend Sub New(ByVal rootDir As File, ByVal batchSize As Integer, ParamArray ByVal validExtensions() As String)
			Me.New(New File(){rootDir}, True, New Random(), batchSize, validExtensions)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected BaseFileIterator(@NonNull File[] rootDirs, boolean recursive, Random rng, int batchSize, String... validExtensions)
		Protected Friend Sub New(ByVal rootDirs() As File, ByVal recursive As Boolean, ByVal rng As Random, ByVal batchSize As Integer, ParamArray ByVal validExtensions() As String)
			Me.batchSize = batchSize
			Me.rng = rng

			list = New CompactHeapStringList()
			For Each rootDir As File In rootDirs
				Dim c As ICollection(Of File) = FileUtils.listFiles(rootDir, validExtensions, recursive)
				If c.Count = 0 Then
					Throw New System.InvalidOperationException("Root directory is empty (no files found) " & (If(validExtensions IsNot Nothing, " (or all files rejected by extension filter)", "")))
				End If
				For Each f As File In c
					list.Add(f.getPath())
				Next f
			Next rootDir

			If rng IsNot Nothing Then
				order = New Integer(list.Count - 1){}
				For i As Integer = 0 To order.Length - 1
					order(i) = i
				Next i
				MathUtils.shuffleArray(order, rng)
			End If
		End Sub

		Public Overrides Function hasNext() As Boolean
			Return partialStored IsNot Nothing OrElse position < list.Count
		End Function

		Public Overrides Function [next]() As T
			If Not hasNext() Then
				Throw New NoSuchElementException("No next element")
			End If

'JAVA TO VB CONVERTER NOTE: The local variable next was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim next_Conflict As T
			If partialStored IsNot Nothing Then
				next_Conflict = partialStored
				partialStored = Nothing
			Else
'JAVA TO VB CONVERTER TODO TASK: The following line contains an assignment within expression that was not extracted by Java to VB Converter:
'ORIGINAL LINE: int nextIdx = (order != null ? order[position++] : position++);
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
				Dim nextIdx As Integer = (If(order IsNot Nothing, order(position++), position))
					position += 1
				next_Conflict = load(New File(list(nextIdx)))
			End If
			If batchSize <= 0 Then
				'Don't recombine, return as-is
				Return next_Conflict
			End If

			If sizeOf(next_Conflict) = batchSize Then
				Return next_Conflict
			End If

			Dim exampleCount As Integer = 0
			Dim toMerge As IList(Of T) = New List(Of T)()
			toMerge.Add(next_Conflict)
			exampleCount += sizeOf(next_Conflict)

			Do While exampleCount < batchSize AndAlso hasNext()
'JAVA TO VB CONVERTER TODO TASK: The following line contains an assignment within expression that was not extracted by Java to VB Converter:
'ORIGINAL LINE: int nextIdx = (order != null ? order[position++] : position++);
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
				Dim nextIdx As Integer = (If(order IsNot Nothing, order(position++), position))
					position += 1
				next_Conflict = load(New File(list(nextIdx)))
				exampleCount += sizeOf(next_Conflict)
				toMerge.Add(next_Conflict)
			Loop

			Dim ret As T = mergeAndStoreRemainder(toMerge)
			applyPreprocessor(ret)
			Return ret
		End Function

		Public Overrides Sub remove()
			Throw New System.NotSupportedException("Not supported")
		End Sub

		Protected Friend Overridable Function mergeAndStoreRemainder(ByVal toMerge As IList(Of T)) As T
			'Could be smaller or larger
			Dim correctNum As IList(Of T) = New List(Of T)()
			Dim remainder As IList(Of T) = New List(Of T)()
			Dim soFar As Integer = 0
			For Each t As T In toMerge
				Dim size As Long = sizeOf(t)

				If soFar + size <= batchSize Then
					correctNum.Add(t)
					soFar += size
				ElseIf soFar < batchSize Then
					'Split and add some
					Dim split As IList(Of T) = Me.split(t)
					If rng IsNot Nothing Then
						Collections.shuffle(split, rng)
					End If
					For Each t2 As T In split
						If soFar < batchSize Then
							correctNum.Add(t2)
							soFar += sizeOf(t2)
						Else
							remainder.Add(t2)
						End If
					Next t2
				Else
					'Don't need any of this
					remainder.Add(t)
				End If
			Next t

			Dim ret As T = merge(correctNum)
			If remainder.Count = 0 Then
				Me.partialStored = Nothing
			Else
				Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.MemoryManager.scopeOutOfWorkspaces()
					Me.partialStored = merge(remainder)
				End Using
			End If

			Return ret
		End Function


		Public Overridable Sub reset()
			position = 0
			If rng IsNot Nothing Then
				MathUtils.shuffleArray(order, rng)
			End If
		End Sub

		Public Overridable Function resetSupported() As Boolean
			Return True
		End Function

		Public Overridable Function asyncSupported() As Boolean
			Return True
		End Function


		Protected Friend MustOverride Function load(ByVal f As File) As T

		Protected Friend MustOverride Function sizeOf(ByVal [of] As T) As Long

		Protected Friend MustOverride Function split(ByVal toSplit As T) As IList(Of T)

		Protected Friend MustOverride Function merge(ByVal toMerge As IList(Of T)) As T

		Protected Friend MustOverride Sub applyPreprocessor(ByVal toPreProcess As T)
	End Class

End Namespace