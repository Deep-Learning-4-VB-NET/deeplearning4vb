Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data
Imports FileUtils = org.apache.commons.io.FileUtils
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.function
Imports org.nd4j.common.primitives

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

Namespace org.nd4j.linalg.util


	Public Class ND4JTestUtils

		Private Sub New()
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Data public static class ComparisonResult
		Public Class ComparisonResult
			Friend allResults As IList(Of Triple(Of File, File, Boolean))
			Friend passed As IList(Of Triple(Of File, File, Boolean))
			Friend failed As IList(Of Triple(Of File, File, Boolean))
			Friend skippedDir1 As IList(Of File)
			Friend skippedDir2 As IList(Of File)
		End Class

		''' <summary>
		''' A function for use with <seealso cref="validateSerializedArrays(File, File, Boolean, BiFunction)"/> using <seealso cref="INDArray.equals(Object)"/>
		''' </summary>
		Public Class EqualsFn
			Implements BiFunction(Of INDArray, INDArray, Boolean)

			Public Overridable Function apply(ByVal i1 As INDArray, ByVal i2 As INDArray) As Boolean?
				Return i1.Equals(i2)
			End Function
		End Class

		''' <summary>
		''' A function for use with <seealso cref="validateSerializedArrays(File, File, Boolean, BiFunction)"/> using <seealso cref="INDArray.equalsWithEps(Object, Double)"/>
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor public static class EqualsWithEpsFn implements org.nd4j.common.function.BiFunction<org.nd4j.linalg.api.ndarray.INDArray,org.nd4j.linalg.api.ndarray.INDArray,Boolean>
		Public Class EqualsWithEpsFn
			Implements BiFunction(Of INDArray, INDArray, Boolean)

			Friend ReadOnly eps As Double

			Public Overridable Function apply(ByVal i1 As INDArray, ByVal i2 As INDArray) As Boolean?
				Return i1.equalsWithEps(i2, eps)
			End Function
		End Class

		''' <summary>
		''' Scan the specified directories for matching files (i.e., same path relative to their respective root directories)
		''' and compare the contents using INDArray.equals (via <seealso cref="EqualsFn"/>
		''' Assumes the saved files represent INDArrays saved with <seealso cref="Nd4j.saveBinary(INDArray, File)"/> </summary>
		''' <param name="dir1">      First directory </param>
		''' <param name="dir2">      Second directory </param>
		''' <param name="recursive"> Whether to search recursively (i.e., include files in subdirectories </param>
		''' <returns> Comparison results </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static ComparisonResult validateSerializedArrays(java.io.File dir1, java.io.File dir2, boolean recursive) throws Exception
		Public Shared Function validateSerializedArrays(ByVal dir1 As File, ByVal dir2 As File, ByVal recursive As Boolean) As ComparisonResult
			Return validateSerializedArrays(dir1, dir2, recursive, New EqualsFn())
		End Function

		''' <summary>
		''' Scan the specified directories for matching files (i.e., same path relative to their respective root directories)
		''' and compare the contents using a provided function.<br>
		''' Assumes the saved files represent INDArrays saved with <seealso cref="Nd4j.saveBinary(INDArray, File)"/> </summary>
		''' <param name="dir1">      First directory </param>
		''' <param name="dir2">      Second directory </param>
		''' <param name="recursive"> Whether to search recursively (i.e., include files in subdirectories </param>
		''' <returns> Comparison results </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static ComparisonResult validateSerializedArrays(java.io.File dir1, java.io.File dir2, boolean recursive, org.nd4j.common.function.BiFunction<org.nd4j.linalg.api.ndarray.INDArray,org.nd4j.linalg.api.ndarray.INDArray,Boolean> evalFn) throws Exception
		Public Shared Function validateSerializedArrays(ByVal dir1 As File, ByVal dir2 As File, ByVal recursive As Boolean, ByVal evalFn As BiFunction(Of INDArray, INDArray, Boolean)) As ComparisonResult
			Dim f1() As File = FileUtils.listFiles(dir1, Nothing, recursive).toArray(New File(){})
			Dim f2() As File = FileUtils.listFiles(dir2, Nothing, recursive).toArray(New File(){})

			Preconditions.checkState(f1.Length > 0, "No files found for directory 1: %s", dir1.getAbsolutePath())
			Preconditions.checkState(f2.Length > 0, "No files found for directory 2: %s", dir2.getAbsolutePath())

			Dim relativized1 As IDictionary(Of String, File) = New Dictionary(Of String, File)()
			Dim relativized2 As IDictionary(Of String, File) = New Dictionary(Of String, File)()

			Dim u As URI = dir1.toURI()
			For Each f As File In f1
				If Not f.isFile() Then
					Continue For
				End If
				Dim relative As String = u.relativize(f.toURI()).getPath()
				relativized1(relative) = f
			Next f

			u = dir2.toURI()
			For Each f As File In f2
				If Not f.isFile() Then
					Continue For
				End If
				Dim relative As String = u.relativize(f.toURI()).getPath()
				relativized2(relative) = f
			Next f

			Dim skipped1 As IList(Of File) = New List(Of File)()
			For Each s As String In relativized1.Keys
				If Not relativized2.ContainsKey(s) Then
					skipped1.Add(relativized1(s))
				End If
			Next s

			Dim skipped2 As IList(Of File) = New List(Of File)()
			For Each s As String In relativized2.Keys
				If Not relativized1.ContainsKey(s) Then
					skipped2.Add(relativized1(s))
				End If
			Next s

			Dim allResults As IList(Of Triple(Of File, File, Boolean)) = New List(Of Triple(Of File, File, Boolean))()
			Dim passed As IList(Of Triple(Of File, File, Boolean)) = New List(Of Triple(Of File, File, Boolean))()
			Dim failed As IList(Of Triple(Of File, File, Boolean)) = New List(Of Triple(Of File, File, Boolean))()
			For Each e As KeyValuePair(Of String, File) In relativized1.SetOfKeyValuePairs()
				Dim file1 As File = e.Value
				Dim file2 As File = relativized2(e.Key)

				If file2 Is Nothing Then
					Continue For
				End If

				Dim i1 As INDArray = Nd4j.readBinary(file1)
				Dim i2 As INDArray = Nd4j.readBinary(file2)
				Dim b As Boolean = evalFn.apply(i1, i2)
				Dim t As New Triple(Of File, File, Boolean)(file1, file2, b)
				allResults.Add(t)
				If b Then
					passed.Add(t)
				Else
					failed.Add(t)
				End If
			Next e

			Dim c As IComparer(Of Triple(Of File, File, Boolean)) = New ComparatorAnonymousInnerClass()

			allResults.Sort(c)
			passed.Sort(c)
			failed.Sort(c)
			skipped1.Sort()
			skipped2.Sort()


			Return New ComparisonResult(allResults, passed, failed, skipped1, skipped2)
		End Function

		Private Class ComparatorAnonymousInnerClass
			Implements IComparer(Of Triple(Of File, File, Boolean))

			Public Function Compare(ByVal o1 As Triple(Of File, File, Boolean), ByVal o2 As Triple(Of File, File, Boolean)) As Integer Implements IComparer(Of Triple(Of File, File, Boolean)).Compare
				Return o1.getFirst().compareTo(o2.getFirst())
			End Function
		End Class
	End Class

End Namespace