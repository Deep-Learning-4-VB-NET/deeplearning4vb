Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports NonNull = lombok.NonNull
Imports At = org.nd4j.autodiff.listeners.At
Imports BaseListener = org.nd4j.autodiff.listeners.BaseListener
Imports Operation = org.nd4j.autodiff.listeners.Operation
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports SameDiffOp = org.nd4j.autodiff.samediff.internal.SameDiffOp
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports OpContext = org.nd4j.linalg.api.ops.OpContext
Imports [Xor] = org.nd4j.linalg.api.ops.impl.transforms.pairwise.bool.Xor
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.nd4j.autodiff.listeners.debugging


	Public Class ArraySavingListener
		Inherits BaseListener

		Protected Friend ReadOnly dir As File
		Protected Friend count As Integer = 0

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ArraySavingListener(@NonNull File dir)
		Public Sub New(ByVal dir As File)

			If Not dir.exists() Then
				dir.mkdir()
			End If

			If dir.listFiles() IsNot Nothing AndAlso dir.listFiles().length > 0 Then
				Throw New System.InvalidOperationException("Directory is not empty: " & dir.getAbsolutePath())
			End If

			Me.dir = dir
		End Sub

		Public Overrides Function isActive(ByVal operation As Operation) As Boolean
			Return True
		End Function


		Public Overrides Sub opExecution(ByVal sd As SameDiff, ByVal at As At, ByVal batch As MultiDataSet, ByVal op As SameDiffOp, ByVal opContext As OpContext, ByVal outputs() As INDArray)
			Dim outNames As IList(Of String) = op.getOutputsOfOp()
			For i As Integer = 0 To outputs.Length - 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: String filename = (count++) + "_" + outNames.get(i).replaceAll("/", "__") + ".bin";
				Dim filename As String = (count) & "_" & outNames(i).replaceAll("/", "__") & ".bin"
					count += 1
				Dim outFile As New File(dir, filename)

				Dim arr As INDArray = outputs(i)
				Try
					Nd4j.saveBinary(arr, outFile)
					Console.WriteLine(outFile.getAbsolutePath())
				Catch e As IOException
					Throw New Exception(e)
				End Try
			Next i
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void compare(java.io.File dir1, java.io.File dir2, double eps) throws Exception
		Public Shared Sub compare(ByVal dir1 As File, ByVal dir2 As File, ByVal eps As Double)
			Dim files1() As File = dir1.listFiles()
			Dim files2() As File = dir2.listFiles()
			Preconditions.checkNotNull(files1, "No files in directory 1: %s", dir1)
			Preconditions.checkNotNull(files2, "No files in directory 2: %s", dir2)
			Preconditions.checkState(files1.Length = files2.Length, "Different number of files: %s vs %s", files1.Length, files2.Length)

			Dim m1 As IDictionary(Of String, File) = toMap(files1)
			Dim m2 As IDictionary(Of String, File) = toMap(files2)

			For Each f As File In files1
				Dim name As String = f.getName()
								Dim tempVar = name.indexOf("_"c) + 1
				Dim varName As String = name.Substring(tempVar, name.Length-4 - (tempVar)) 'Strip "x_" and ".bin"
				Dim f2 As File = m2(varName)

				Dim arr1 As INDArray = Nd4j.readBinary(f)
				Dim arr2 As INDArray = Nd4j.readBinary(f2)

				'TODO String arrays won't work here!
				Dim eq As Boolean = arr1.equalsWithEps(arr2, eps)
				If eq Then
					Console.WriteLine("Equals: " & varName.replaceAll("__", "/"))
				Else
					If arr1.dataType() = DataType.BOOL Then
						Dim [xor] As INDArray = Nd4j.exec(New [Xor](arr1, arr2))
						Dim count As Integer = [xor].castTo(DataType.INT).sumNumber().intValue()
						Console.WriteLine("FAILS: " & varName.replaceAll("__", "/") & " - boolean, # differences = " & count)
						Console.WriteLine(vbTab & f.getAbsolutePath())
						Console.WriteLine(vbTab & f2.getAbsolutePath())
						[xor].close()
					Else
						Dim [sub] As INDArray = arr1.sub(arr2)
						Dim diff As INDArray = Nd4j.math_Conflict.abs([sub])
						Dim maxDiff As Double = diff.maxNumber().doubleValue()
						Console.WriteLine("FAILS: " & varName.replaceAll("__", "/") & " - max difference = " & maxDiff)
						Console.WriteLine(vbTab & f.getAbsolutePath())
						Console.WriteLine(vbTab & f2.getAbsolutePath())
						[sub].close()
						diff.close()
					End If
				End If
				arr1.close()
				arr2.close()
			Next f
		End Sub

		Private Shared Function toMap(ByVal files() As File) As IDictionary(Of String, File)
			Dim ret As IDictionary(Of String, File) = New Dictionary(Of String, File)()
			For Each f As File In files
				Dim name As String = f.getName()
								Dim tempVar = name.indexOf("_"c) + 1
				Dim varName As String = name.Substring(tempVar, name.Length - 4 - (tempVar)) 'Strip "x_" and ".bin"
				ret(varName) = f
			Next f
			Return ret
		End Function
	End Class

End Namespace